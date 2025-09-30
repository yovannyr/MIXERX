# MIXERX - Professional DJ Software

**Ultra-low latency DJ software built with .NET 9 for Windows and macOS**

## 🎯 Key Features

- **Ultra-Low Latency:** <10ms audio processing pipeline
- **Cross-Platform:** Native Windows and macOS support
- **Multi-Deck System:** 4 independent audio decks
- **MIDI Controller Support:** JavaScript-based mapping system
- **Track Library:** SQLite-based music library with search
- **Modern UI:** Avalonia-based responsive interface

## 🏗️ Architecture

- **Separated Processes:** UI and Audio Engine for optimal performance
- **Lock-Free Audio:** Real-time audio processing without GC pressure
- **Sandboxed Scripting:** Safe JavaScript controller mappings
- **Cross-Platform Audio:** WASAPI (Windows) and CoreAudio (macOS)

## 🚀 Quick Start

### Prerequisites
- .NET 9 SDK
- Windows 10+ or macOS 10.15+

### Build & Run
```bash
# Clone repository
git clone <repository-url>
cd MIXERX

# Build solution
dotnet build

# Run Audio Engine
dotnet run --project src/MIXERX.Engine

# Run UI (separate terminal)
dotnet run --project src/MIXERX.UI
```

### Testing
```bash
# Run unit tests
dotnet test tests/MIXERX.Core.Tests
```

## 📁 Project Structure

```
MIXERX/
├── src/
│   ├── MIXERX.Core/          # Shared interfaces and models
│   ├── MIXERX.Engine/        # Real-time audio processing
│   └── MIXERX.UI/            # Avalonia user interface
├── tests/
│   └── MIXERX.Core.Tests/    # Unit tests
├── mappings/                 # Controller mapping examples
└── docs/                     # Architecture documentation
```

## 🎛️ Controller Mapping

MIXERX uses JavaScript for safe, hot-reloadable controller mappings:

```javascript
function onMidiMessage(msg) {
    const deck1 = getDeck(1);
    
    if (msg.isNoteOn(0x10)) {
        deck1.playPause();
    }
    
    if (msg.isCC(0x30)) {
        const tempo = 0.5 + (msg.value * 1.5);
        deck1.setTempo(tempo);
    }
}
```

## 📊 Performance Targets

- **Latency:** <10ms controller-to-audio
- **Stability:** 24h continuous operation
- **CPU Usage:** <60% (4 decks + effects)
- **Startup Time:** <3 seconds

## 🔧 Development Status

**Core Features Implemented:**
- ✅ Audio engine architecture
- ✅ Multi-deck playback system
- ✅ MIDI controller support
- ✅ Track library management
- ✅ Cross-platform UI

**In Development:**
- Audio codec integration
- Effects processing
- Advanced sync features

## 📄 License

[License information to be determined]

## 🤝 Contributing

[Contributing guidelines to be established]
