# MIXERX Design Document

## Architecture Reference

See [Architecture Document](./architecture.md) for high-level system context and architecture decisions.

## Technology Stack (Final Implementation)

- **Runtime:** .NET 9 - Latest LTS with modern C# 12/13 features
- **UI Framework:** Avalonia UI 11.0 - Cross-platform MVVM with ReactiveUI
- **Audio Processing:** NAudio - .NET audio library with WASAPI/CoreAudio support
- **Graphics:** SkiaSharp - GPU-accelerated rendering for waveforms and UI
- **IPC:** Google.Protobuf + SharedMemory - Efficient UI↔Engine communication (designed)
- **Scripting:** Jint 3.0 - Sandboxed JavaScript engine for controller mappings
- **Database:** Microsoft.Data.Sqlite + EF Core - Track library with full-text search
- **MIDI:** Melanchall.DryWetMidi - Comprehensive MIDI device support

## System Architecture (Implemented)

### Process Separation Design

```
┌─────────────────┐    IPC     ┌──────────────────┐
│   MIXERX.UI     │◄──────────►│  MIXERX.Engine   │
│   (Avalonia)    │  Protobuf  │  (Real-time)     │
│   - GUI         │SharedMemory│  - Audio I/O     │
│   - Controller  │            │  - DSP Pipeline  │
│   - Library     │            │  - Deck Control  │
└─────────────────┘            └──────────────────┘
```

**Rationale:** Isolates real-time audio processing from UI to prevent GC pauses affecting audio (_Requirements: REQ-1, REQ-2_)

## Components and Interfaces

### Audio Engine Process (MIXERX.Engine)

**Purpose:** Real-time audio processing with <10ms latency guarantee

**Key Interfaces:**
```csharp
public interface IAudioEngine
{
    Task<bool> StartAsync(AudioConfig config);
    Task StopAsync();
    void LoadTrack(int deckId, string filePath);
    void SetTempo(int deckId, double bpm);
    void SetPosition(int deckId, TimeSpan position);
    AudioMetrics GetMetrics();
}

public interface IAudioDecoder : IDisposable
{
    AudioData LoadFile(string path);
    int ReadSamples(float[] buffer, int offset, int count);
    int Read(float[] buffer, int offset, int count);
    bool Seek(TimeSpan position);
    int SampleRate { get; }
    int Channels { get; }
    TimeSpan Duration { get; }
}
```

**Audio Codec Support:**
- WAV: Native decoder (WavDecoder)
- MP3/FLAC/AAC/OGG/M4A: FFmpeg-based decoder (FFmpegAudioDecoder)
- Extensible through IAudioDecoder interface
- Centralized decoder selection via AudioDecoderFactory

public class AudioConfig
{
    public int SampleRate { get; set; } = 48000;
    public int BufferSize { get; set; } = 128;
    public AudioApi PreferredApi { get; set; }
}
```

**Performance Requirements:**
- Lock-free audio path using `Span<T>` and pre-allocated buffers
- High-priority thread scheduling (MMCSS on Windows, Audio Workgroups on macOS)
- No GC allocations in real-time path
- NativeAOT compilation for minimal startup overhead

### UI Application (MIXERX.UI)

**Purpose:** User interface, controller management, library operations

**Key Components:**
```csharp
public interface IDeckViewModel
{
    ICommand PlayPauseCommand { get; }
    ICommand LoadTrackCommand { get; }
    ObservableProperty<double> Position { get; }
    ObservableProperty<double> Tempo { get; }
    ObservableProperty<bool> IsPlaying { get; }
}

public interface ILibraryService
{
    Task<IEnumerable<Track>> SearchAsync(string query);
    Task<Track> AnalyzeTrackAsync(string filePath);
    Task ImportDirectoryAsync(string path);
}
```

### Audio Pipeline Design

**Signal Flow:**
```
File → Decoder → Resampler → Timestretch → EQ → FX → Mixer → Output
                     ↑           ↑         ↑    ↑     ↑
                 Deck Ctrl   Tempo Ctrl   User  User  Master
```

**Core Pipeline Interface:**
```csharp
public interface IAudioNode
{
    void Process(Span<float> input, Span<float> output, int sampleCount);
    void SetParameter(string name, float value);
}

public class AudioGraph
{
    public void AddNode(IAudioNode node, string id);
    public void ConnectNodes(string sourceId, string targetId);
    public void Process(int sampleCount);
}
```

### Cross-Platform Audio Abstraction

**Audio API Wrapper:**
```csharp
public interface IAudioDriver
{
    bool Initialize(AudioConfig config);
    void Start(AudioCallback callback);
    void Stop();
    AudioDeviceInfo[] GetDevices();
}

public delegate void AudioCallback(Span<float> input, Span<float> output);

// Platform implementations
public class WasapiDriver : IAudioDriver { }
public class CoreAudioDriver : IAudioDriver { }
public class AsioDriver : IAudioDriver { } // Optional
```

### Controller Mapping System

**JavaScript Mapping Engine:**
```csharp
public interface IControllerMapper
{
    void LoadMapping(string mappingScript);
    void ProcessMidiMessage(MidiMessage message);
    void SendFeedback(string controlId, object value);
}

// Example mapping structure
public class ControllerMapping
{
    public string Name { get; set; }
    public string VendorId { get; set; }
    public string ProductId { get; set; }
    public Dictionary<string, ControlDefinition> Controls { get; set; }
    public string ScriptContent { get; set; }
}
```

**Mapping Script API:**
```javascript
// Example controller mapping
export function onMidiMessage(msg) {
    const deck = getDeck(1);
    
    if (msg.isNoteOn(0x10)) {
        deck.playPause();
    }
    
    if (msg.isCC(0x20)) {
        deck.setTempo(msg.value / 127.0 * 2.0); // 0.5x to 2.0x
    }
}

export function onDeckStateChange(deckId, state) {
    if (state.isPlaying) {
        sendMidi(0x90, 0x10, 127); // LED on
    } else {
        sendMidi(0x80, 0x10, 0);   // LED off
    }
}
```

### Library and Database Design

**Data Model:**
```csharp
public class Track
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public string Title { get; set; }
    public string Artist { get; set; }
    public TimeSpan Duration { get; set; }
    public double? Bpm { get; set; }
    public string? Key { get; set; }
    public double? ReplayGain { get; set; }
    public DateTime LastModified { get; set; }
    public byte[]? WaveformData { get; set; }
}

public class Crate
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Track> Tracks { get; set; }
    public bool IsSmartCrate { get; set; }
    public string? SmartCrateQuery { get; set; }
}
```

**Database Schema:**
```sql
-- Core tables with FTS5 for fast search
CREATE TABLE tracks (
    id INTEGER PRIMARY KEY,
    file_path TEXT UNIQUE NOT NULL,
    title TEXT,
    artist TEXT,
    album TEXT,
    duration_ms INTEGER,
    bpm REAL,
    key TEXT,
    replay_gain REAL,
    last_modified INTEGER,
    waveform_blob BLOB
);

CREATE VIRTUAL TABLE tracks_fts USING fts5(
    title, artist, album, content='tracks'
);
```

### IPC Protocol Design

**Message Types:**
```protobuf
syntax = "proto3";

message EngineCommand {
    oneof command {
        LoadTrackCommand load_track = 1;
        PlayCommand play = 2;
        SetTempoCommand set_tempo = 3;
        SetPositionCommand set_position = 4;
    }
}

message LoadTrackCommand {
    int32 deck_id = 1;
    string file_path = 2;
}

message EngineStatus {
    repeated DeckStatus decks = 1;
    AudioMetrics metrics = 2;
}

message DeckStatus {
    int32 deck_id = 1;
    bool is_playing = 2;
    double position_seconds = 3;
    double tempo_ratio = 4;
    float level_left = 5;
    float level_right = 6;
}
```

**Shared Memory Layout:**
```csharp
[StructLayout(LayoutKind.Sequential)]
public struct SharedAudioMetrics
{
    public float CpuUsage;
    public int BufferUnderruns;
    public long SamplesProcessed;
    public double Latency;
}
```

## Performance Optimizations

### Real-Time Audio Path

**Lock-Free Design:**
- Ring buffers for audio data transfer
- Atomic operations for control parameters
- Pre-allocated buffer pools
- SIMD intrinsics for DSP operations

**Memory Management:**
```csharp
public class AudioBufferPool
{
    private readonly ConcurrentQueue<float[]> _buffers;
    private readonly int _bufferSize;
    
    public float[] Rent() => _buffers.TryDequeue(out var buffer) 
        ? buffer 
        : new float[_bufferSize];
        
    public void Return(float[] buffer) => _buffers.Enqueue(buffer);
}
```

### GPU-Accelerated Waveforms

**Waveform Rendering:**
```csharp
public interface IWaveformRenderer
{
    void RenderWaveform(Span<float> audioData, SKCanvas canvas, SKRect bounds);
    void UpdateTiles(WaveformTile[] tiles);
}

public class GpuWaveformRenderer : IWaveformRenderer
{
    // OpenGL-based tile rendering for smooth zooming
    // Pre-computed LOD levels for different zoom factors
}
```

## Error Handling and Recovery

### Audio Engine Watchdog

```csharp
public class AudioEngineWatchdog
{
    private readonly Timer _watchdogTimer;
    
    public void StartMonitoring()
    {
        _watchdogTimer.Start(TimeSpan.FromSeconds(1));
    }
    
    private void CheckEngineHealth()
    {
        if (DetectXRun() || DetectEngineHang())
        {
            RestartAudioEngine();
        }
    }
}
```

### Graceful Degradation

- Automatic fallback from ASIO to WASAPI on driver issues
- Reduced quality mode when CPU usage exceeds 80%
- Emergency stop with fade-out on critical errors

## Security Considerations

### Controller Script Sandboxing

```csharp
public class SecureScriptEngine
{
    private readonly Jint.Engine _jsEngine;
    
    public SecureScriptEngine()
    {
        _jsEngine = new Jint.Engine(options =>
        {
            options.LimitRecursion(100);
            options.TimeoutInterval(TimeSpan.FromMilliseconds(100));
            options.AllowClr(false); // No .NET access
        });
    }
}
```

### File System Access

- Sandboxed file access for audio files only
- No execution of external binaries from mappings
- Validation of all file paths and extensions

## Testing Strategy

### Unit Testing

**Audio Processing Tests:**
```csharp
[Test]
public void AudioNode_ProcessesSilence_OutputsSilence()
{
    var node = new EqNode();
    var input = new float[1024];
    var output = new float[1024];
    
    node.Process(input, output, 1024);
    
    Assert.That(output, Is.All.EqualTo(0.0f));
}
```

### Integration Testing

**Latency Testing:**
```csharp
[Test]
public async Task AudioEngine_MeasuresLatency_UnderTarget()
{
    var engine = new AudioEngine();
    await engine.StartAsync(new AudioConfig { BufferSize = 128 });
    
    var latency = await MeasureRoundTripLatency();
    
    Assert.That(latency, Is.LessThan(TimeSpan.FromMilliseconds(10)));
}
```

### Performance Testing

- Continuous latency monitoring in CI
- CPU usage benchmarks with reference tracks
- Memory allocation profiling for GC pressure
- 24-hour stability tests

## Deployment Architecture

### Application Structure

```
MIXERX/
├── MIXERX.UI.exe           # Main UI application
├── MIXERX.Engine.exe       # Audio engine process
├── runtimes/
│   ├── win-x64/native/     # Windows native libraries
│   └── osx-x64/native/     # macOS native libraries
├── mappings/               # Controller mappings
└── assets/                 # UI resources
```

### Platform-Specific Packaging

**Windows (MSIX):**
- Code-signed with EV certificate
- Bundled VC++ Redistributable
- Windows Store compatible

**macOS (.pkg/.dmg):**
- Notarized and hardened runtime
- Universal binary (Intel + Apple Silicon)
- Proper entitlements for audio/MIDI access

## Quality Assurance

### Automated Testing

- Unit tests for all audio processing components
- Integration tests for IPC communication
- Performance regression tests
- Controller mapping validation

### Manual Testing

- Real-world DJ workflow testing
- Hardware compatibility testing
- Stress testing with large libraries
- Accessibility testing with screen readers

## Success Metrics Implementation

- **Latency Monitoring:** Built-in latency measurement tools
- **Stability Tracking:** Crash reporting and uptime metrics
- **Performance Profiling:** CPU/memory usage dashboards
- **Usability Metrics:** Task completion time measurement tools

---

This design provides the foundation for implementing MIXERX with the performance, stability, and usability requirements specified in the requirements document.
