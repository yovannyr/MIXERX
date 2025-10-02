# Entwicklungsplan: MIXERX (main branch)

*Erstellt am 2025-10-02 durch Vibe Feature MCP*
*Workflow: [epcc](https://mrsimpson.github.io/responsible-vibe-mcp/workflows/epcc)*

## Ziel
Track Waveform Analysis - Ermöglicht visuelle Analyse der Track-Struktur.

## Feature-Prioritätenliste (Reihenfolge)
1. ✅ Waveform Visualization - ABGESCHLOSSEN
2. ✅ Effects Processing - ABGESCHLOSSEN
3. ✅ **Beat Detection & Auto-Sync** - ABGESCHLOSSEN (VOLLSTÄNDIG)
4. ✅ MP3 Export - ABGESCHLOSSEN
5. ✅ Advanced Loop Features - ABGESCHLOSSEN
6. ⏳ **Track Waveform Analysis** ← AKTUELL

## Explore

### Phasen-Eintrittskriterien
- [x] Entwicklungs-Workflow gestartet

### Aufgaben

### Abgeschlossen
- [x] Bestehende Waveform-Infrastruktur analysiert
- [x] Track-Struktur-Analyse-Optionen geprüft
- [x] Minimal Scope definiert

### Erkenntnisse
**Bestehende Infrastruktur:**
- ✅ WaveformControl vorhanden (zeigt Waveform an)
- ✅ WaveformAnalyzer vorhanden (Peak-Detection)
- ✅ WaveformData wird bereits an UI gesendet
- ✅ Cue Points und Beat Markers in WaveformControl

**Was "Track Waveform Analysis" bedeutet:**
- Track-Struktur erkennen (Intro, Verse, Chorus, Breakdown, Outro)
- Energy-Level über Zeit analysieren
- Visuelle Segmente in Waveform anzeigen
- Hot Cues automatisch an wichtigen Punkten setzen

**MINIMAL SCOPE:**
- ✅ Energy-Level Analyse (bereits in BpmAnalyzer vorhanden)
- ✅ Visuelle Energy-Overlay in WaveformControl
- ✅ Farbcodierung: Niedrig (blau) → Mittel (grün) → Hoch (rot)
- ❌ KEINE automatische Struktur-Erkennung (zu komplex)
- ❌ KEINE Auto-Cue-Points (später)
- ❌ KEINE Phrase-Detection (später)

## Plan

### Phasen-Eintrittskriterien
- [x] Anforderungen klar definiert (MINIMAL SCOPE)
- [x] Technischer Ansatz festgelegt

### Implementierungsstrategie

**Ansatz:** Energy-Level Analyse + visuelles Overlay in WaveformControl.

**Komponenten:**
1. **WaveformAnalyzer** - Energy-Level pro Sample berechnen (zusätzlich zu Peaks)
2. **WaveformControl** - Energy-Overlay rendern (Farbverlauf)
3. **Deck.cs** - Energy-Daten mit Waveform senden

**Technische Details:**
- Energy = RMS (Root Mean Square) pro Segment
- Waveform in Segmente teilen (z.B. 100 Segmente)
- Energy-Level normalisieren (0.0 - 1.0)
- Farbcodierung: 0.0-0.3 (blau), 0.3-0.7 (grün), 0.7-1.0 (rot)
- Overlay als halbtransparente Farbbalken unter Waveform

**Zero-Break:**
- WaveformAnalyzer erweitern (nicht ersetzen)
- WaveformControl Render-Methode erweitern
- Keine IPC-Änderungen nötig (Energy in WaveformData integrieren)

### Detaillierter Implementierungsplan

#### 1. WaveformAnalyzer erweitern
**Datei:** `src/MIXERX.Engine/Analysis/WaveformAnalyzer.cs`
- CalculateEnergyLevels() Methode hinzufügen
- Energy pro Segment berechnen (RMS)

#### 2. Deck.cs Energy-Daten
**Datei:** `src/MIXERX.Engine/Deck.cs`
- Energy-Levels mit Waveform senden (als zweites Array)

#### 3. WaveformControl Rendering
**Datei:** `src/MIXERX.UI/Controls/WaveformControl.cs`
- EnergyLevels Property hinzufügen
- Energy-Overlay in Render() zeichnen

### Aufgaben

### Abgeschlossen
- [x] Implementierungsstrategie definiert

## Code

### Phasen-Eintrittskriterien
- [x] Plan vollständig
- [x] Technischer Ansatz bestätigt

### Aufgaben

### Abgeschlossen
- [x] 1. WaveformAnalyzer: CalculateEnergyLevels() Methode hinzugefügt
- [x] 2. WaveformControl: EnergyLevels Property + Energy-Overlay Rendering
- [x] 3. Build getestet (0 Errors)

## Commit

### Phasen-Eintrittskriterien
- [x] Loop Features implementiert
- [x] Build erfolgreich
- [x] Keine Regression

### Aufgaben

### Abgeschlossen
- [x] Git commit erstellt (9e7738f)
- [x] Änderungen gepusht
**Geänderte Dateien:**
- `src/MIXERX.Core/IpcProtocol.cs`:
  - ExitLoop, LoopStatus MessageTypes hinzugefügt
  - SetLoopMessage, ExitLoopMessage, LoopStatusMessage Records

- `src/MIXERX.Engine/Deck.cs`:
  - GetLoopInfo() Methode für Loop-Status

- `src/MIXERX.Engine/AudioEngine.cs`:
  - SetAutoLoop(deckId, beats) Methode
  - ExitLoop(deckId) Methode

- `src/MIXERX.Engine/IpcServer.cs`:
  - SetLoop Message Handler
  - ExitLoop Message Handler

- `src/MIXERX.UI/Services/EngineService.cs`:
  - SetAutoLoopAsync(deckId, beats)
  - ExitLoopAsync(deckId)
  - LoopStatusReceived Event

- `src/MIXERX.UI/ViewModels/DeckViewModel.cs`:
  - SetAutoLoop Command → EngineService verbunden
  - ExitLoop Command → EngineService verbunden

**Technische Details:**
- LoopEngine bereits vorhanden, nur Integration
- Auto-Loop: 1, 2, 4, 8 beats (UI bereits vorhanden)
- Zero-Break: LoopEngine unverändert

### Abgeschlossen

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
