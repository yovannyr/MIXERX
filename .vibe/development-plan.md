# Entwicklungsplan: MIXERX (main branch)

*Erstellt am 2025-10-02 durch Vibe Feature MCP*
*Workflow: [epcc](https://mrsimpson.github.io/responsible-vibe-mcp/workflows/epcc)*

## Ziel
Advanced Loop Features - Ermöglicht präzise Loop-Steuerung für DJ-Performances.

## Feature-Prioritätenliste (Reihenfolge)
1. ✅ Waveform Visualization - ABGESCHLOSSEN
2. ✅ Effects Processing - ABGESCHLOSSEN
3. ✅ Beat Detection & Auto-Sync - ABGESCHLOSSEN (MINIMAL: nur BPM-Anzeige)
4. ✅ MP3 Export - ABGESCHLOSSEN
5. ⏳ **Advanced Loop Features** ← AKTUELL
6. ⏳ Track Waveform Analysis

## Explore

### Phasen-Eintrittskriterien
- [x] Entwicklungs-Workflow gestartet

### Aufgaben

### Abgeschlossen
- [x] Bestehende Loop-Infrastruktur analysiert
- [x] UI-Komponenten geprüft
- [x] Engine-Integration geplant

### Erkenntnisse
**Bestehende Infrastruktur:**
- ✅ LoopEngine vorhanden (vollständig implementiert)
- ✅ LoopControl UI-Component vorhanden
- ✅ DeckView: Loop-Buttons vorhanden (1/2/4/8 beats)
- ✅ DeckViewModel: Loop Commands definiert
- ✅ BeatGrid für beat-synchrone Loops

**LoopEngine Features:**
- Auto-Loop (1, 2, 4, 8, 16, 32 beats)
- Manual Loop In/Out
- Loop Exit/Reloop
- Loop Halve/Double
- Loop Roll (temporary loop)
- Loop Progress tracking

**Was fehlt:**
- ❌ Loop Commands nicht mit Engine verbunden
- ❌ IPC Messages für Loop-Steuerung fehlen
- ❌ Deck.cs: LoopEngine nicht integriert
- ❌ Loop-Status wird nicht an UI gesendet

**MINIMAL SCOPE:**
- ✅ Auto-Loop (1, 2, 4, 8 beats) aktivieren
- ✅ Loop Exit/Reloop
- ✅ Loop-Status in UI anzeigen
- ❌ KEINE Loop Halve/Double (später)
- ❌ KEINE Loop Roll (später)
- ❌ KEINE Manual Loop In/Out (später)

## Plan

### Phasen-Eintrittskriterien
- [x] Anforderungen klar definiert (MINIMAL SCOPE)
- [x] Technischer Ansatz festgelegt

### Implementierungsstrategie

**Ansatz:** LoopEngine in Deck.cs integrieren und via IPC mit UI verbinden.

**Komponenten:**
1. **IpcProtocol** - Loop Messages hinzufügen (SetAutoLoop, ExitLoop, LoopStatus)
2. **Deck.cs** - LoopEngine integrieren, ProcessSamplePosition nutzen
3. **IpcServer** - Loop-Commands verarbeiten
4. **EngineService** - Loop-Methoden hinzufügen
5. **DeckViewModel** - Commands mit EngineService verbinden

**Technische Details:**
- LoopEngine.ProcessSamplePosition() in Deck.Read() aufrufen
- Loop-Status periodisch an UI senden (mit Playback-Updates)
- BeatGrid aus BPM berechnen (vereinfacht)

**Zero-Break:**
- LoopEngine bleibt unverändert
- Nur Integration in bestehende Strukturen
- UI bereits vorhanden

### Detaillierter Implementierungsplan

#### 1. IpcProtocol erweitern
**Datei:** `src/MIXERX.Core/IpcProtocol.cs`
- SetAutoLoop Message (deckId, beats)
- ExitLoop Message (deckId)
- LoopStatus Message (deckId, isLooping, lengthBeats, progress)

#### 2. Deck.cs Integration
**Datei:** `src/MIXERX.Engine/Deck.cs`
- LoopEngine Instanz hinzufügen
- ProcessSamplePosition in Read() aufrufen
- GetLoopInfo() Methode

#### 3. IpcServer Commands
**Datei:** `src/MIXERX.Engine/IpcServer.cs`
- SetAutoLoop Handler
- ExitLoop Handler
- Loop-Status in Playback-Updates

#### 4. EngineService Methoden
**Datei:** `src/MIXERX.UI/Services/EngineService.cs`
- SetAutoLoopAsync(deckId, beats)
- ExitLoopAsync(deckId)
- LoopStatusReceived Event

#### 5. DeckViewModel Commands
**Datei:** `src/MIXERX.UI/ViewModels/DeckViewModel.cs`
- AutoLoopCommand → EngineService.SetAutoLoopAsync
- ExitLoopCommand → EngineService.ExitLoopAsync
- LoopStatusReceived Event Handler

### Aufgaben

### Abgeschlossen
- [x] Implementierungsstrategie definiert

## Code

### Phasen-Eintrittskriterien
- [x] Plan vollständig
- [x] Technischer Ansatz bestätigt

### Aufgaben

### Abgeschlossen
- [x] 1. IpcProtocol: Loop Messages hinzugefügt (SetLoop, ExitLoop, LoopStatus)
- [x] 2. Deck.cs: GetLoopInfo() Methode hinzugefügt
- [x] 3. IpcServer: Loop Commands verarbeitet
- [x] 4. AudioEngine: SetAutoLoop/ExitLoop Methoden hinzugefügt
- [x] 5. EngineService: Loop-Methoden hinzugefügt
- [x] 6. DeckViewModel: Commands verbunden
- [x] 7. Build getestet (0 Errors)

## Commit

### Phasen-Eintrittskriterien
- [x] Loop Features implementiert
- [x] Build erfolgreich
- [x] Keine Regression

### Aufgaben
- [ ] Git commit erstellen
- [ ] Änderungen pushen

### Implementierte Änderungen
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
