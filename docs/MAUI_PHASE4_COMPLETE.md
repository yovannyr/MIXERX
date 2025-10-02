# MAUI Migration - Phase 4: Architecture Decisions

**Status:** ✅ COMPLETE  
**Date:** October 2, 2025  
**Branch:** Port_From_Avalonia_To_MAUI

---

## 📋 Phase 4 Objectives

Establish architectural patterns and platform-specific integrations for the MAUI application.

---

## ✅ Completed Architecture Decisions

### 4.1 MVVM Pattern ✅
**Decision:** Use CommunityToolkit.Mvvm for MVVM implementation

**Implementation:**
- `[ObservableObject]` for ViewModels
- `[ObservableProperty]` for automatic property change notification
- `[RelayCommand]` for command generation
- Clean, minimal boilerplate code

**Applied in:**
- `MainViewModel.cs`
- `DeckViewModel.cs`
- `LibraryViewModel.cs`
- `SettingsViewModel.cs`

---

### 4.2 Navigation ✅
**Decision:** Use Shell Navigation for app-wide navigation

**Implementation:**
- `AppShell.xaml` with FlyoutItems
- Route-based navigation
- 3 main routes: `main`, `library`, `settings`

**Shell Structure:**
```xml
<Shell>
  <FlyoutItem Title="DJ">
    <ShellContent Route="main" />
  </FlyoutItem>
  <FlyoutItem Title="Library">
    <ShellContent Route="library" />
  </FlyoutItem>
  <FlyoutItem Title="Settings">
    <ShellContent Route="settings" />
  </FlyoutItem>
</Shell>
```

---

### 4.3 Audio Engine Integration ✅
**Decision:** Separate process for desktop, in-process for mobile

**Architecture:**
- **Desktop (Windows/macOS):** Separate MIXERX.Engine process
- **Mobile (iOS/Android):** In-process audio engine
- **IPC:** Named pipes for desktop communication
- **Shared Memory:** Waveform data transfer

**Implementation:**
- `IAudioEngineService` interface
- `AudioEngineService` with platform detection
- Automatic process management
- Named pipe client for IPC

**Platform Detection:**
```csharp
#if ANDROID || IOS
    _useInProcessEngine = true;  // Mobile
#else
    _useInProcessEngine = false; // Desktop
#endif
```

---

### 4.4 Platform-Specific Audio Services ✅
**Decision:** Abstract platform audio APIs behind common interface

**Interface:**
```csharp
public interface IPlatformAudioService
{
    Task<IEnumerable<AudioDevice>> GetOutputDevicesAsync();
    Task<IEnumerable<AudioDevice>> GetInputDevicesAsync();
    Task SetOutputDeviceAsync(string deviceId);
    Task<int> GetOptimalBufferSizeAsync();
    bool SupportsLowLatency { get; }
}
```

**Platform Implementations:**

| Platform | Implementation | Audio API | Latency Target |
|----------|---------------|-----------|----------------|
| Windows | `WindowsAudioService` | WASAPI Exclusive | 3-6ms (256 samples) |
| macOS | `MacAudioService` | CoreAudio | 3-6ms (256 samples) |
| iOS | `iOSAudioService` | AVAudioEngine | 5-11ms (256 samples) |
| Android | `AndroidAudioService` | AAudio/OpenSL ES | 10-20ms (480 samples) |

**Conditional Compilation:**
```csharp
#if WINDOWS
    builder.Services.AddSingleton<IPlatformAudioService, WindowsAudioService>();
#elif MACCATALYST
    builder.Services.AddSingleton<IPlatformAudioService, MacAudioService>();
#elif ANDROID
    builder.Services.AddSingleton<IPlatformAudioService, AndroidAudioService>();
#elif IOS
    builder.Services.AddSingleton<IPlatformAudioService, iOSAudioService>();
#endif
```

---

## 📊 Files Created

| File | Purpose | Lines |
|------|---------|-------|
| `Services/AudioEngineService.cs` | Audio engine IPC management | 95 |
| `Services/IPlatformAudioService.cs` | Platform audio abstraction | 12 |
| `Platforms/Windows/WindowsAudioService.cs` | Windows WASAPI implementation | 40 |
| `Platforms/MacCatalyst/MacAudioService.cs` | macOS CoreAudio implementation | 40 |
| `Platforms/Android/AndroidAudioService.cs` | Android AAudio implementation | 40 |
| `Platforms/iOS/iOSAudioService.cs` | iOS AVAudioEngine implementation | 40 |

**Total:** 6 files, ~267 lines of code

---

## 🔧 Dependency Injection Setup

**MauiProgram.cs Configuration:**
```csharp
// Audio services
builder.Services.AddSingleton<IAudioEngineService, AudioEngineService>();
builder.Services.AddSingleton<IPlatformAudioService, [Platform]AudioService>();

// ViewModels
builder.Services.AddTransient<MainViewModel>();
builder.Services.AddTransient<LibraryViewModel>();
builder.Services.AddTransient<SettingsViewModel>();

// Views
builder.Services.AddTransient<MainPage>();
builder.Services.AddTransient<LibraryPage>();
builder.Services.AddTransient<SettingsPage>();
```

---

## 🎯 Architecture Benefits

### 1. **Separation of Concerns**
- UI layer (MAUI) separate from audio processing (Engine)
- Platform-specific code isolated in Platforms/ folders
- Clean interfaces for testability

### 2. **Cross-Platform Consistency**
- Same API across all platforms
- Platform differences hidden behind abstractions
- Conditional compilation only in service registration

### 3. **Performance Optimization**
- Desktop: Separate process prevents UI blocking
- Mobile: In-process reduces IPC overhead
- Platform-optimal buffer sizes

### 4. **Maintainability**
- Clear architectural boundaries
- Easy to add new platforms
- Testable service interfaces

---

## 🔍 Design Patterns Used

1. **Dependency Injection** - All services registered in DI container
2. **Strategy Pattern** - Platform-specific audio services
3. **Facade Pattern** - AudioEngineService hides IPC complexity
4. **Factory Pattern** - Platform service selection at runtime
5. **MVVM Pattern** - Clean separation of UI and logic

---

## 📝 Implementation Notes

### Desktop Audio Engine Process
- Engine executable must be in same directory as MAUI app
- Named pipe: `MIXERX_Engine`
- Connection timeout: 5 seconds
- Automatic process cleanup on app exit

### Mobile In-Process Engine
- TODO: Implement in-process audio engine for mobile
- Will use platform-specific audio APIs directly
- No IPC overhead
- Shared memory for waveform data

### Platform Audio APIs
- **Windows:** NAudio wrapper for WASAPI
- **macOS:** AVFoundation bindings
- **iOS:** AVFoundation (same as macOS)
- **Android:** Android.Media.AudioManager

---

## ✅ Phase 4 Success Criteria

- [x] MVVM pattern established with CommunityToolkit.Mvvm
- [x] Shell navigation configured
- [x] Audio engine service created with IPC support
- [x] Platform-specific audio services implemented
- [x] Conditional compilation for platform selection
- [x] Dependency injection configured
- [x] All services registered in MauiProgram.cs
- [x] MainViewModel updated to use new services

**Phase 4 Status: ✅ COMPLETE**

---

## 🚀 Next Steps (Phase 5)

### Week 1-2: Foundation
- [x] Create MIXERX.MAUI project ✅
- [x] Install Syncfusion & dependencies ✅
- [x] Setup Shell navigation ✅
- [x] Create basic page structure ✅
- [ ] Test on Windows/macOS

### Week 3-4: Core Controls
- [x] Port WaveformControl (SkiaSharp) ✅
- [x] Port DeckView ✅
- [x] Port transport controls ✅
- [ ] Test waveform rendering performance

### Week 5-6: Deck Features (NEXT)
- [ ] Port hot cues
- [ ] Port loop controls
- [ ] Port tempo/pitch controls
- [ ] Port sync functionality

---

## 🎯 Overall MAUI Migration Progress

| Phase | Status | Completion |
|-------|--------|------------|
| Phase 1: Planning | ✅ Complete | 100% |
| Phase 2: Foundation | ✅ Complete | 100% |
| Phase 3: UI Components | ✅ Complete | 100% |
| **Phase 4: Architecture** | **✅ Complete** | **100%** |
| Phase 5: Migration Execution | 🔄 In Progress | 40% |
| Phase 6: New Features | ⏳ Pending | 0% |
| Phase 7: Mobile Optimization | ⏳ Pending | 0% |
| Phase 8: Testing | ⏳ Pending | 0% |
| Phase 9: Deployment | ⏳ Pending | 0% |
| Phase 10: Success Metrics | ⏳ Pending | 0% |

**Overall Progress: 40% (4/10 phases complete)**

---

**Next Phase:** Phase 5 (Week 5-6) - Deck Features Implementation  
**Estimated Duration:** 2 weeks  
**Key Deliverables:** Hot cues, Loop controls, Tempo/Pitch, Sync
