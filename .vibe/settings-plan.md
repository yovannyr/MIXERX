# MIXERX Settings Implementation Plan

## Analysis of Implemented Features

### 1. **Audio Engine Features**
- Audio driver selection (WASAPI/CoreAudio)
- Sample rate (44.1kHz, 48kHz, 96kHz)
- Buffer size (latency control)
- Master volume
- Cue volume
- Audio output device selection

### 2. **Deck Features**
- Tempo range (-8% to +8%, configurable)
- Keylock on/off
- Sync mode (auto-sync enabled/disabled)
- Waveform zoom level
- Waveform color scheme
- Energy overlay visibility
- Beat markers visibility
- Cue points color scheme

### 3. **Effects Features**
- Reverb wet/dry default
- Delay mix default
- Filter cutoff default
- Effects bypass on track load

### 4. **Loop Features**
- Auto-loop default length (1, 2, 4, 8 beats)
- Loop quantize (beat-snap)
- Loop roll enabled

### 5. **Recording Features**
- Default recording format (WAV/MP3)
- MP3 bitrate (128, 192, 256, 320 kbps)
- Recording directory
- Auto-delete WAV after MP3 conversion

### 6. **Library Features**
- Library scan directories
- Auto-import on startup
- Show iTunes library
- Track analysis on import (BPM, Key)
- Played track color
- Library text size
- Column visibility

### 7. **Display Features**
- BPM decimal places (1 or 2)
- Key notation (Camelot, Standard, Open Key)
- Tempo matching display
- EQ colored waveforms
- Hi-res screen display
- Maximum screen updates (FPS)
- Full screen mode

### 8. **MIDI Features**
- MIDI device selection
- Controller mapping file
- MIDI learn mode
- MIDI clock sync

### 9. **Performance Features**
- Hot cue count (4, 8, 16)
- Hot cue colors
- Performance pad layout

### 10. **Privacy & Telemetry**
- Send usage data
- Crash reports
- Anonymous statistics

---

## Settings Groups (Tab Pages)

### Tab 1: **Audio**
- Audio Driver
- Sample Rate
- Buffer Size
- Output Device
- Master Volume
- Cue Volume
- Latency Display

### Tab 2: **Decks**
- Tempo Range
- Keylock Default
- Sync Mode
- Waveform Zoom
- Waveform Colors
- Energy Overlay
- Beat Markers
- Cue Point Colors

### Tab 3: **Effects & Loops**
- Reverb Default
- Delay Default
- Filter Default
- Effects Bypass on Load
- Auto-Loop Length
- Loop Quantize
- Loop Roll

### Tab 4: **Library & Display**
- Library Directories
- Auto-Import
- iTunes Integration
- Track Analysis
- Played Track Color
- Library Text Size
- BPM Decimal Places
- Key Notation
- Column Visibility

### Tab 5: **Recording**
- Default Format
- MP3 Bitrate
- Recording Directory
- Auto-Delete WAV
- File Naming Pattern

### Tab 6: **MIDI & Controllers**
- MIDI Device
- Controller Mapping
- MIDI Learn
- MIDI Clock Sync

### Tab 7: **Performance**
- Hot Cue Count
- Hot Cue Colors
- Pad Layout
- Screen Updates (FPS)
- Hi-Res Display
- Full Screen Default

### Tab 8: **Privacy**
- Usage Data
- Crash Reports
- Anonymous Stats

---

## Settings Storage

**Location:** `%APPDATA%/MIXERX/settings.json` (Windows) or `~/.config/MIXERX/settings.json` (Linux/Mac)

**Format:** JSON

**Structure:**
```json
{
  "audio": {
    "driver": "WASAPI",
    "sampleRate": 48000,
    "bufferSize": 512,
    "outputDevice": "default",
    "masterVolume": 0.85,
    "cueVolume": 0.75
  },
  "decks": {
    "tempoRange": 8,
    "keylockDefault": false,
    "syncMode": "manual",
    "waveformZoom": 1.0,
    "waveformColors": "default",
    "energyOverlay": true,
    "beatMarkers": true
  },
  "effects": {
    "reverbWet": 0.3,
    "delayMix": 0.5,
    "filterCutoff": 0.5,
    "bypassOnLoad": false
  },
  "loops": {
    "defaultLength": 4,
    "quantize": true,
    "rollEnabled": false
  },
  "recording": {
    "format": "mp3",
    "mp3Bitrate": 192,
    "directory": "%USERPROFILE%/Music/MIXERX",
    "autoDeleteWav": true
  },
  "library": {
    "directories": [],
    "autoImport": true,
    "showItunes": false,
    "analyzeOnImport": true,
    "playedTrackColor": "blue",
    "textSize": 12
  },
  "display": {
    "bpmDecimals": 1,
    "keyNotation": "camelot",
    "tempoMatching": false,
    "eqWaveforms": false,
    "hiRes": false,
    "maxFps": 30
  },
  "midi": {
    "device": "none",
    "mappingFile": "",
    "learnMode": false,
    "clockSync": false
  },
  "performance": {
    "hotCueCount": 4,
    "hotCueColors": ["#ff0044", "#ff8800", "#00ff88", "#0088ff"],
    "padLayout": "default",
    "fullScreenDefault": false
  },
  "privacy": {
    "sendUsageData": true,
    "crashReports": true,
    "anonymousStats": true
  }
}
```

---

## Implementation Tasks

### Phase 1: Settings Model & Storage
- [ ] Create `Settings.cs` model with all properties
- [ ] Create `SettingsService.cs` for load/save
- [ ] Implement JSON serialization
- [ ] Create default settings
- [ ] Add settings directory creation

### Phase 2: Settings UI (8 Tabs)
- [ ] Update SettingsWindow.axaml with 8 tabs
- [ ] Create Audio tab content
- [ ] Create Decks tab content
- [ ] Create Effects & Loops tab content
- [ ] Create Library & Display tab content
- [ ] Create Recording tab content
- [ ] Create MIDI tab content
- [ ] Create Performance tab content
- [ ] Create Privacy tab content

### Phase 3: Settings ViewModel
- [ ] Create SettingsViewModel
- [ ] Bind all settings properties
- [ ] Implement Apply/Cancel/Reset logic
- [ ] Add validation

### Phase 4: Integration
- [ ] Load settings on app startup
- [ ] Apply settings to AudioEngine
- [ ] Apply settings to Decks
- [ ] Apply settings to UI
- [ ] Save settings on Apply

### Phase 5: Testing
- [ ] Test settings persistence
- [ ] Test default values
- [ ] Test validation
- [ ] Test cross-platform paths
