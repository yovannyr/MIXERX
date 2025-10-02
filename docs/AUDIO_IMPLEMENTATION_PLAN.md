# MIXERX - Kritische Audio-Features Implementation Plan
## Zero-Break, Inkrementell, TDD-Driven

### 🎯 Ziel: Funktionsfähige DJ-Software in 5 Iterationen

---

## Iteration 1: Audio Foundation (Tag 1-2)
**Ziel:** Basis Audio-System ohne Breaking Changes

### 1.1 Test-First: Audio Driver Interface
```csharp
// tests/MIXERX.Engine.Tests/AudioDriverTests.cs - NEU
[Test] public void AudioDriver_Initialize_ReturnsTrue()
[Test] public void AudioDriver_Start_WithValidConfig_Succeeds()
[Test] public void AudioDriver_ProcessBuffer_CallsCallback()
```

### 1.2 Minimal Implementation
```csharp
// src/MIXERX.Engine/AudioDrivers.cs - ERWEITERN
public class MockAudioDriver : IAudioDriver // Für Tests
public class WasapiDriver : IAudioDriver   // Windows Stub → Real
public class CoreAudioDriver : IAudioDriver // macOS Stub → Real
```

### 1.3 Zero-Break Integration
- Bestehende AudioEngine.cs bleibt unverändert
- Nur interne Driver-Implementierung erweitern
- Fallback auf Mock-Driver bei Fehlern

**Deliverable:** Audio-Driver läuft, aber spielt noch keine echten Files

---

## Iteration 2: Audio Buffer System (Tag 2-3)
**Ziel:** Lock-free Audio-Pipeline

### 2.1 Test-First: Buffer Management
```csharp
// tests/MIXERX.Engine.Tests/AudioBufferTests.cs - NEU
[Test] public void AudioBuffer_Write_DoesNotBlock()
[Test] public void AudioBuffer_Read_ReturnsValidData()
[Test] public void AudioBuffer_Underrun_HandlesGracefully()
```

### 2.2 Minimal Implementation
```csharp
// src/MIXERX.Engine/Audio/AudioBuffer.cs - NEU
public unsafe class LockFreeAudioBuffer
{
    private fixed float _buffer[8192]; // Minimal ring buffer
    private volatile int _writePos, _readPos;
}
```

### 2.3 Zero-Break Integration
- AudioEngine.cs erweitern, nicht ersetzen
- Bestehende Deck-API bleibt gleich
- Buffer als interne Implementierung

**Deliverable:** Audio-Pipeline funktioniert, aber spielt noch Silence

---

## Iteration 3: WAV Decoder (Tag 3-4)
**Ziel:** Erste echte Audio-Wiedergabe

### 3.1 Test-First: WAV Support
```csharp
// tests/MIXERX.Engine.Tests/WavDecoderTests.cs - NEU
[Test] public void WavDecoder_LoadFile_ReturnsAudioData()
[Test] public void WavDecoder_InvalidFile_ThrowsException()
[Test] public void WavDecoder_ReadSamples_ReturnsCorrectCount()
```

### 3.2 Minimal Implementation
```csharp
// src/MIXERX.Engine/Codecs/WavDecoder.cs - NEU
public class WavDecoder : IAudioDecoder
{
    public AudioData LoadFile(string path) // Nur 44.1kHz/16bit
    public float[] ReadSamples(int count)
}
```

### 3.3 Zero-Break Integration
- Deck.cs erweitern um LoadTrack(string path)
- Bestehende Play/Pause API bleibt gleich
- Fallback auf Silence bei Decode-Fehlern

**Deliverable:** WAV-Files können abgespielt werden

---

## Iteration 4: IPC Communication (Tag 4-5)
**Ziel:** UI ↔ Engine Verbindung

### 4.1 Test-First: IPC System
```csharp
// tests/MIXERX.Core.Tests/IpcTests.cs - NEU
[Test] public void IpcClient_Connect_Succeeds()
[Test] public void IpcClient_SendCommand_ReceivesResponse()
[Test] public void IpcServer_HandleCommand_ExecutesCorrectly()
```

### 4.2 Minimal Implementation
```csharp
// src/MIXERX.Core/IPC/IpcClient.cs - NEU
public class IpcClient
{
    public async Task<T> SendCommand<T>(IpcCommand cmd)
    public event Action<AudioState> StateChanged;
}
```

### 4.3 Zero-Break Integration
- UI ViewModels erweitern, nicht ersetzen
- Engine läuft weiterhin standalone
- IPC als optionale Verbindung

**Deliverable:** UI kann Engine steuern (Play/Pause/Load)

---

## Iteration 5: File Integration (Tag 5-6)
**Ziel:** Vollständige DJ-Funktionalität

### 5.1 Test-First: File System
```csharp
// tests/MIXERX.UI.Tests/FileServiceTests.cs - NEU
[Test] public void FileService_OpenDialog_ReturnsValidPath()
[Test] public void FileService_LoadTrack_UpdatesDeck()
[Test] public void FileService_InvalidFile_ShowsError()
```

### 5.2 Minimal Implementation
```csharp
// src/MIXERX.UI/Services/FileService.cs - NEU
public class FileService
{
    public async Task<string?> OpenAudioFile()
    public async Task LoadTrackToDeck(string path, int deckId)
}
```

### 5.3 Zero-Break Integration
- ViewModels erweitern bestehende Commands
- Placeholder-Pfade durch echte Dialogs ersetzen
- Graceful Error-Handling

**Deliverable:** Vollständig funktionsfähige DJ-Software

---

## 🔧 TDD-Workflow pro Iteration

### Red-Green-Refactor Cycle:
1. **RED:** Test schreiben (fails)
2. **GREEN:** Minimal Code für Pass
3. **REFACTOR:** Code optimieren
4. **INTEGRATE:** Zero-break in Main

### Continuous Integration:
```bash
# Nach jeder Änderung
dotnet test                    # Alle Tests müssen passen
dotnet build                   # Build muss erfolgreich sein
dotnet run --project Engine    # Engine muss starten
dotnet run --project UI        # UI muss starten
```

---

## 📦 Deliverables pro Iteration

| Iteration | Deliverable | Tests | Funktionalität |
|-----------|-------------|-------|----------------|
| 1 | Audio Driver | 3 Tests | Driver initialisiert |
| 2 | Buffer System | 6 Tests | Audio-Pipeline läuft |
| 3 | WAV Decoder | 9 Tests | WAV-Wiedergabe |
| 4 | IPC System | 12 Tests | UI steuert Engine |
| 5 | File Integration | 15 Tests | Vollständige DJ-App |

---

## 🚀 Zero-Break Garantien

### Bestehende APIs bleiben:
- `IAudioEngine` Interface unverändert
- `Deck` Public Methods unverändert  
- `ViewModel` Properties unverändert
- `IpcProtocol` Messages erweitert, nicht geändert

### Fallback-Strategien:
- Audio-Fehler → Silence statt Crash
- IPC-Fehler → Local Mode
- File-Fehler → Error Dialog
- Driver-Fehler → Mock Driver

### Backward Compatibility:
- Alte Tests bleiben funktional
- Bestehende Mappings funktionieren
- UI bleibt responsive
- Engine startet immer

---

## 📊 Risiko-Mitigation

### Kritische Pfade:
1. **Audio Driver:** Platform-spezifisch → Mock als Fallback
2. **IPC Communication:** Process-übergreifend → In-Process Fallback  
3. **File I/O:** Async Operations → Timeout + Cancel
4. **Memory Management:** Unsafe Code → Safe Wrapper

### Rollback-Plan:
- Jede Iteration ist eigenständig deploybar
- Feature-Flags für neue Funktionen
- Automatische Tests verhindern Regression
- Git-Branches für sichere Integration

---

## 🎯 Success Metrics

### Nach Iteration 3:
- ✅ WAV-Files spielen ab
- ✅ Alle bestehenden Tests passen
- ✅ UI startet ohne Fehler
- ✅ Engine läuft stabil

### Nach Iteration 5:
- ✅ Vollständige DJ-Funktionalität
- ✅ <10ms Audio-Latenz
- ✅ 15+ Unit Tests
- ✅ Zero Breaking Changes

**Geschätzte Gesamtzeit: 6 Tage**
**Risiko: NIEDRIG (durch inkrementelle Entwicklung)**
