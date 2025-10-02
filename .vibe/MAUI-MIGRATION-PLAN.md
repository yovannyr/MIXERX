# MIXERX: Avalonia → .NET MAUI Migration Plan

## Executive Summary

**Goal:** Port MIXERX from Avalonia UI to .NET MAUI to achieve Serato DJ Pro feature parity while maintaining cross-platform support (Windows, macOS, iOS, Android).

**Current State:** 
- ✅ 6 core features implemented in Avalonia
- ✅ Professional DJ interface (MainWindow2)
- ✅ Comprehensive settings system
- ✅ Audio engine with effects, loops, sync

**Target State:**
- 🎯 Full .NET MAUI implementation
- 🎯 40 Serato-parity features (F1-F40)
- 🎯 Syncfusion MAUI Toolkit integration
- 🎯 Mobile support (iOS/Android)

---

## Phase 1: Analysis & Architecture (Current Phase)

### 1.1 Feature Gap Analysis

**Implemented in Avalonia (6 features):**
1. ✅ Waveform Visualization
2. ✅ Effects Processing (Reverb, Delay, Filter)
3. ✅ Beat Detection & Auto-Sync
4. ✅ MP3 Export
5. ✅ Advanced Loop Features
6. ✅ Track Waveform Analysis

**Required for Serato Parity (40 features from arc42):**

**CRITICAL (Must Have - Phase 2):**
- F1: Multi-Deck Mixing & Practice Mode (2/4 decks)
- F3: File Format & Streaming Support (Apple Music, Beatport, SoundCloud, Spotify, Tidal)
- F5: Sync & Tempo Control (Beat Sync, Smart Sync, Tempo Sync)
- F6: Loops & Loop Roll
- F7: Cue Points & Hot Cues (8 per track)
- F8: Keylock & Pitch Control
- F11: Effects (DJ-FX with iZotope)
- F12: Sampler (8 slots, 32 samples across 4 banks)
- F13: Recorder
- F14: MIDI/HID Mapping & Remapping

**HIGH PRIORITY (Phase 3):**
- F4: Stems Separation (Vocals, Melody, Bass, Drums)
- F9: Slip Mode & Censor
- F10: Beat Jump & Quantize
- F15: Ableton Link & Network Sync
- F21: Playlists & History
- F26: Smart Crates & Library Management
- F29: Key Detection & Harmonic Mixing
- F30: Slicer & Performance Pads

**MEDIUM PRIORITY (Phase 4):**
- F16: DVS Control (Time-Code Vinyl/CDJs)
- F18: Cue Automation/Flip
- F20: Remote Control & Play Mode
- F24: Track Play Count & Library Indicators
- F25: Day Mode & High-Visibility UI
- F27: Library & Display Enhancements
- F28: Display Modes & Deck Layouts
- F32: Prepare Window & Crate Workflow
- F33: Cloud Sync & Settings

**LOW PRIORITY (Phase 5+):**
- F17: Video Mixing (Optional)
- F19: Pitch 'n Time Algorithms (Optional)
- F31: Offline Streaming Locker
- F34-F40: Performance, Security, Modularity, Tutorials

---

## Phase 2: MAUI Foundation Setup

### 2.1 Project Structure

```
MIXERX/
├── src/
│   ├── MIXERX.Core/              # Keep as-is (shared logic)
│   ├── MIXERX.Engine/            # Keep as-is (audio engine)
│   ├── MIXERX.MAUI/              # NEW: MAUI UI project
│   │   ├── Platforms/
│   │   │   ├── Android/
│   │   │   ├── iOS/
│   │   │   ├── MacCatalyst/
│   │   │   └── Windows/
│   │   ├── Views/
│   │   ├── ViewModels/
│   │   ├── Controls/
│   │   ├── Resources/
│   │   └── MauiProgram.cs
│   └── MIXERX.UI/                # DEPRECATED: Avalonia (keep for reference)
```

### 2.2 Dependencies

**Add to MIXERX.MAUI:**
```xml
<PackageReference Include="Syncfusion.Maui.Toolkit" Version="27.*" />
<PackageReference Include="CommunityToolkit.Maui" Version="9.*" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.*" />
<PackageReference Include="SkiaSharp.Views.Maui.Controls" Version="2.*" />
```

**Keep from Engine:**
```xml
<PackageReference Include="NAudio" Version="2.*" />
<PackageReference Include="FFMpegCore" Version="5.*" />
<PackageReference Include="TagLibSharp" Version="2.*" />
<PackageReference Include="Melanchall.DryWetMidi" Version="7.*" />
```

### 2.3 Syncfusion MAUI Toolkit Components

**For DJ Interface:**
- `SfCartesianChart` - Waveform visualization
- `SfSlider` - Tempo, volume, effect controls
- `SfButton` - Hot cues, transport controls
- `SfTabView` - Settings tabs, deck switching
- `SfListView` - Track library
- `SfDataGrid` - Advanced library view
- `SfBusyIndicator` - Loading states
- `SfEffectsView` - Touch feedback
- `SfSegmentedControl` - Mode switching

---

## Phase 3: UI Component Migration Strategy

### 3.1 Avalonia → MAUI Mapping

| Avalonia Component | MAUI Equivalent | Notes |
|-------------------|-----------------|-------|
| `Window` | `ContentPage` / `Shell` | Use Shell for navigation |
| `UserControl` | `ContentView` | Custom controls |
| `Button` | `Button` / `SfButton` | Syncfusion for advanced styling |
| `TextBlock` | `Label` | Direct mapping |
| `TextBox` | `Entry` / `Editor` | Single/multi-line |
| `Slider` | `Slider` / `SfSlider` | Syncfusion for DJ controls |
| `ComboBox` | `Picker` / `SfComboBox` | Syncfusion for advanced features |
| `CheckBox` | `CheckBox` / `Switch` | Platform-specific |
| `DataGrid` | `CollectionView` / `SfDataGrid` | Syncfusion for library |
| `TabControl` | `TabbedPage` / `SfTabView` | Syncfusion recommended |
| `Menu` | `MenuBar` / `FlyoutMenu` | Shell FlyoutMenu |
| `Border` | `Border` / `Frame` | MAUI has native Border |
| `Canvas` | `AbsoluteLayout` / `GraphicsView` | For custom drawing |
| Custom `WaveformControl` | `GraphicsView` + SkiaSharp | Redraw with Skia |

### 3.2 Critical Custom Controls to Port

**Priority 1:**
1. **WaveformControl** → `GraphicsView` with SkiaSharp
   - Peak visualization
   - Energy overlay
   - Playhead indicator
   - Beat markers
   - Cue points

2. **LoopControl** → `SfButton` + Custom rendering
   - Loop progress indicator
   - Beat length display

3. **DeckView** → `ContentView` composite
   - Waveform
   - Transport controls
   - Hot cues
   - Loop controls
   - Tempo slider

**Priority 2:**
4. **MixerControl** → `ContentView` with sliders
   - EQ (High, Mid, Low)
   - Filter
   - Gain
   - Crossfader

5. **LibraryView** → `SfDataGrid`
   - Track list
   - Column sorting
   - Search
   - Playlists

---

## Phase 4: Architecture Decisions

### 4.1 MVVM Pattern

**Use CommunityToolkit.Mvvm:**
```csharp
[ObservableObject]
public partial class DeckViewModel
{
    [ObservableProperty]
    private string trackName;
    
    [ObservableProperty]
    private float[] waveformData;
    
    [RelayCommand]
    private async Task PlayPause()
    {
        // Implementation
    }
}
```

### 4.2 Navigation

**Use Shell Navigation:**
```xml
<Shell>
    <FlyoutItem Title="DJ">
        <ShellContent Route="main" ContentTemplate="{DataTemplate views:MainPage}" />
    </FlyoutItem>
    <FlyoutItem Title="Library">
        <ShellContent Route="library" ContentTemplate="{DataTemplate views:LibraryPage}" />
    </FlyoutItem>
    <FlyoutItem Title="Settings">
        <ShellContent Route="settings" ContentTemplate="{DataTemplate views:SettingsPage}" />
    </FlyoutItem>
</Shell>
```

### 4.3 Audio Engine Integration

**Keep IPC Architecture:**
- Engine runs in separate process (Windows/macOS)
- Named pipes for communication
- Shared memory for waveform data
- Mobile: In-process audio engine

### 4.4 Platform-Specific Code

**Audio Drivers:**
```csharp
#if WINDOWS
    using NAudio.Wave;
#elif MACCATALYST || IOS
    using AVFoundation;
#elif ANDROID
    using Android.Media;
#endif
```

---

## Phase 5: Migration Execution Plan

### 5.1 Step-by-Step Migration

**Week 1-2: Foundation**
- [ ] Create MIXERX.MAUI project
- [ ] Install Syncfusion & dependencies
- [ ] Setup Shell navigation
- [ ] Create basic page structure
- [ ] Test on Windows/macOS

**Week 3-4: Core Controls**
- [ ] Port WaveformControl (SkiaSharp)
- [ ] Port DeckView
- [ ] Port transport controls
- [ ] Test waveform rendering performance

**Week 5-6: Deck Features**
- [ ] Port hot cues
- [ ] Port loop controls
- [ ] Port tempo/pitch controls
- [ ] Port sync functionality

**Week 7-8: Mixer & Effects**
- [ ] Port mixer controls (EQ, filter, gain)
- [ ] Port crossfader
- [ ] Port effects UI
- [ ] Test audio routing

**Week 9-10: Library**
- [ ] Port library view (SfDataGrid)
- [ ] Port search functionality
- [ ] Port playlist management
- [ ] Test large libraries (10k+ tracks)

**Week 11-12: Settings & Polish**
- [ ] Port settings (8 tabs)
- [ ] Port settings persistence
- [ ] Apply theme/styling
- [ ] Test on all platforms

---

## Phase 6: New Features (Serato Parity)

### 6.1 Streaming Integration (F3)

**Services to Integrate:**
- Apple Music API
- Beatport Streaming API
- SoundCloud API
- Spotify Web API
- Tidal API

**Implementation:**
```csharp
public interface IStreamingService
{
    Task<bool> AuthenticateAsync();
    Task<IEnumerable<Track>> SearchAsync(string query);
    Task<Stream> GetAudioStreamAsync(string trackId);
    Task<IEnumerable<Playlist>> GetPlaylistsAsync();
}
```

### 6.2 Stems Separation (F4)

**Technology:**
- Spleeter (Python) via process
- Demucs (PyTorch)
- OpenUnmix
- GPU acceleration (CUDA/Metal)

**Implementation:**
```csharp
public interface IStemsEngine
{
    Task<StemsData> SeparateAsync(string audioFile);
    void SetStemVolume(StemType stem, float volume);
    void MuteStem(StemType stem, bool mute);
}

public enum StemType
{
    Vocals,
    Melody,
    Bass,
    Drums
}
```

### 6.3 Sampler (F12)

**32 Samples across 4 Banks:**
```csharp
public class SamplerEngine
{
    private SampleSlot[,] _samples = new SampleSlot[4, 8]; // 4 banks, 8 slots
    
    public void LoadSample(int bank, int slot, string filePath);
    public void TriggerSample(int bank, int slot, PlayMode mode);
    public void SetSampleSync(int bank, int slot, bool sync);
}

public enum PlayMode
{
    Trigger,  // Play once
    Hold,     // Play while held
    OnOff     // Toggle on/off
}
```

### 6.4 Smart Crates (F26)

**Rule-Based Playlists:**
```csharp
public class SmartCrate
{
    public string Name { get; set; }
    public List<CrateRule> Rules { get; set; }
    public CrateLogic Logic { get; set; } // AND/OR
}

public class CrateRule
{
    public string Field { get; set; } // BPM, Key, Artist, Genre, etc.
    public RuleOperator Operator { get; set; } // Equals, Contains, GreaterThan, etc.
    public object Value { get; set; }
}
```

---

## Phase 7: Mobile Optimization

### 7.1 Touch-Optimized UI

**Gestures:**
- Swipe: Deck switching
- Pinch: Waveform zoom
- Long press: Set cue point
- Double tap: Play/pause
- Two-finger swipe: Crossfader

### 7.2 Mobile-Specific Features

**iOS/Android:**
- Bluetooth MIDI controller support
- AirPlay/Chromecast output
- Background audio playback
- Lock screen controls
- Haptic feedback

---

## Phase 8: Testing Strategy

### 8.1 Unit Tests
- Audio engine components
- DSP algorithms
- Sync logic
- Library management

### 8.2 Integration Tests
- IPC communication
- Streaming services
- MIDI/HID controllers
- File format support

### 8.3 Performance Tests
- Waveform rendering (60 FPS)
- Audio latency (<10ms)
- Large library loading
- Stems separation speed

### 8.4 Platform Tests
- Windows 10/11
- macOS 12+ (Intel & Apple Silicon)
- iOS 15+
- Android 10+

---

## Phase 9: Deployment

### 9.1 Distribution Channels

**Desktop:**
- Microsoft Store (Windows)
- Mac App Store (macOS)
- Direct download (.exe/.dmg)

**Mobile:**
- Apple App Store (iOS)
- Google Play Store (Android)

### 9.2 Licensing Model

**Tiers:**
1. **Free:** 2-deck mode, basic features
2. **Pro:** 4-deck mode, effects, sampler ($99/year)
3. **Ultimate:** Stems, streaming, DVS ($199/year)

---

## Phase 10: Success Metrics

### 10.1 Technical KPIs
- ✅ Audio latency <10ms
- ✅ UI rendering 60 FPS
- ✅ Startup time <3 seconds
- ✅ Memory usage <500MB (4 decks)
- ✅ CPU usage <60% (4 decks + effects)

### 10.2 Feature Parity
- ✅ 40/40 Serato features implemented
- ✅ All file formats supported
- ✅ 5+ streaming services integrated
- ✅ 100+ MIDI controllers supported

### 10.3 User Adoption
- 🎯 10,000 downloads (Month 1)
- 🎯 50,000 active users (Month 6)
- 🎯 4.5+ star rating (App Stores)

---

## Risk Mitigation

| Risk | Impact | Mitigation |
|------|--------|------------|
| MAUI performance issues | High | Benchmark early, optimize critical path |
| Streaming API limitations | Medium | Implement fallbacks, cache aggressively |
| Stems separation latency | High | GPU acceleration, pre-processing option |
| Mobile audio latency | High | Use native audio APIs, optimize buffer sizes |
| Cross-platform bugs | Medium | Extensive testing, platform-specific code |
| Licensing costs (Syncfusion) | Low | Budget allocated, ROI justified |

---

## Timeline Summary

| Phase | Duration | Deliverable |
|-------|----------|-------------|
| 1. Analysis | 1 week | This document |
| 2. MAUI Setup | 2 weeks | Working MAUI project |
| 3. Core Migration | 8 weeks | Ported UI components |
| 4. New Features | 12 weeks | Serato parity features |
| 5. Mobile Optimization | 4 weeks | iOS/Android support |
| 6. Testing & Polish | 4 weeks | Production-ready |
| 7. Deployment | 2 weeks | Published apps |

**Total: ~33 weeks (8 months)**

---

## Next Steps

1. ✅ Create this migration plan
2. ⏳ Create MIXERX.MAUI project structure
3. ⏳ Install Syncfusion MAUI Toolkit
4. ⏳ Port WaveformControl (proof of concept)
5. ⏳ Port MainWindow2 to MAUI Shell
6. ⏳ Test on Windows & macOS
7. ⏳ Begin feature implementation

---

**Status:** Ready to begin Phase 2 (MAUI Foundation Setup)
**Branch:** `Port_From_Avalonia_To_MAUI`
**Last Updated:** 2025-10-02
