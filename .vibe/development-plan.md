# Entwicklungsplan: MIXERX (main branch)

*Erstellt am 2025-10-02 durch Vibe Feature MCP*
*Workflow: [epcc](https://mrsimpson.github.io/responsible-vibe-mcp/workflows/epcc)*

## Ziel
MP3 Export - Ermöglicht Export von Aufnahmen im MP3-Format (zusätzlich zu WAV).

## Feature-Prioritätenliste (Reihenfolge)
1. ✅ Waveform Visualization - ABGESCHLOSSEN
2. ✅ Effects Processing - ABGESCHLOSSEN
3. ✅ Beat Detection & Auto-Sync - ABGESCHLOSSEN (MINIMAL: nur BPM-Anzeige)
4. ⏳ **MP3 Export** ← AKTUELL
5. ⏳ Advanced Loop Features
6. ⏳ Track Waveform Analysis

## Explore

### Phasen-Eintrittskriterien
- [x] Entwicklungs-Workflow gestartet

### Aufgaben

### Abgeschlossen
- [x] Bestehende Recording-Infrastruktur analysiert
- [x] MP3-Encoding-Optionen geprüft (NAudio, FFMpegCore)
- [x] UI-Integration geplant

### Erkenntnisse
**Bestehende Infrastruktur:**
- ✅ RecordingEngine vorhanden (WAV-only, 16-bit PCM)
- ✅ UI Recording Controls vorhanden (REC/STOP Buttons)
- ✅ MainWindowViewModel: StartRecording/StopRecording Commands
- ✅ FFMpegCore bereits installiert (Version 5.2.0)
- ✅ File Picker Dialog für Speicherort

**Aktueller Flow:**
1. User klickt REC → File Picker öffnet sich
2. User wählt Speicherort (nur .wav)
3. EngineService.StartRecordingAsync(filePath)
4. RecordingEngine schreibt WAV
5. User klickt STOP → StopRecording

**Was fehlt:**
- MP3 als Export-Option im File Picker
- Post-Processing: WAV → MP3 Konvertierung nach Recording
- Bitrate-Auswahl (128/192/320 kbps)

**MINIMAL SCOPE:**
- ✅ MP3 Export nach Recording (nicht während)
- ✅ FFMpegCore für Konvertierung nutzen
- ✅ Standard-Bitrate: 192 kbps (gute Qualität/Größe Balance)
- ❌ KEINE Echtzeit-MP3-Encoding (zu komplex)
- ❌ KEINE Bitrate-Auswahl UI (später)

## Plan

### Phasen-Eintrittskriterien
- [x] Anforderungen klar definiert (MINIMAL SCOPE)
- [x] Technischer Ansatz festgelegt

### Implementierungsstrategie

**Ansatz:** Post-Recording WAV → MP3 Konvertierung mit FFMpegCore.

**Komponenten:**
1. **MainWindowViewModel** - File Picker mit MP3-Option erweitern
2. **RecordingEngine** - Bleibt unverändert (WAV-only)
3. **Mp3Converter** - Neue Klasse für FFMpegCore-Integration

**Technische Details:**
- FFMpegCore.FFMpeg.Convert() für WAV → MP3
- Bitrate: 192 kbps (Standard)
- Nach Konvertierung: WAV optional löschen
- Progress-Feedback optional (später)

**Zero-Break:**
- Keine Engine-Änderungen
- RecordingEngine bleibt WAV-only
- Nur UI/ViewModel Update
- MP3 als zusätzliche Option

### Detaillierter Implementierungsplan

#### 1. Mp3Converter Klasse
**Datei:** `src/MIXERX.Engine/Audio/Mp3Converter.cs` (NEU)
- ConvertWavToMp3Async(string wavPath, string mp3Path, int bitrate = 192)
- FFMpegCore Integration

#### 2. MainWindowViewModel Update
**Datei:** `src/MIXERX.UI/ViewModels/MainWindowViewModel.cs`
- File Picker: MP3 als Option hinzufügen
- Nach StopRecording: Wenn MP3 gewählt → Konvertierung
- Optional: WAV nach Konvertierung löschen

### Aufgaben

### Abgeschlossen
- [x] Implementierungsstrategie definiert
- [x] MINIMAL SCOPE festgelegt

## Code

### Phasen-Eintrittskriterien
- [x] Plan vollständig
- [x] Technischer Ansatz bestätigt

### Aufgaben

### Abgeschlossen
- [x] 1. Mp3Converter Klasse erstellt
- [x] 2. MainWindowViewModel: File Picker mit MP3-Option erweitert
- [x] 3. MainWindowViewModel: Post-Recording MP3-Konvertierung
- [x] 4. Build getestet (0 Errors)

## Commit

### Phasen-Eintrittskriterien
- [x] MP3 Export implementiert
- [x] Build erfolgreich
- [x] Keine Regression

### Aufgaben
- [ ] Git commit erstellen
- [ ] Änderungen pushen

### Implementierte Änderungen
**Neue Dateien:**
- `src/MIXERX.Engine/Audio/Mp3Converter.cs` - FFMpegCore Integration für WAV→MP3

**Geänderte Dateien:**
- `src/MIXERX.UI/ViewModels/MainWindowViewModel.cs`:
  - File Picker: MP3 als primäre Option (Standard)
  - Recording-Flow: WAV → MP3 Konvertierung nach Stop
  - Temporäre WAV-Datei wird nach Konvertierung gelöscht

**Technische Details:**
- FFMpegCore mit libmp3lame Codec
- Bitrate: 192 kbps (Standard)
- Zero-Break: RecordingEngine unverändert

### Abgeschlossen

## Wichtige Entscheidungen

### MINIMAL SCOPE
- **KEINE Beat-Detection** - zu komplex für jetzt
- **KEIN Auto-Sync** - später
- **NUR BPM-Anzeige** aus Metadaten
- **Erweiterbar** - BpmAnalyzer bleibt für später

### Zero-Break Strategie
- Keine Engine-Änderungen
- UI bereits vorhanden
- Nur DeckViewModel Update

## Notizen
- BpmAnalyzer existiert für zukünftige Beat-Detection
- SyncEngine existiert für zukünftiges Auto-Sync
- Jetzt: Nur Metadaten-BPM anzeigen

---
*Dieser Plan wird vom LLM gepflegt. Tool-Antworten geben Anleitung für die aktuelle Phase.*
