# Entwicklungsplan: MIXERX (main branch)

*Erstellt am 2025-10-02 durch Vibe Feature MCP*
*Workflow: [epcc](https://mrsimpson.github.io/responsible-vibe-mcp/workflows/epcc)*

## Ziel
Beat Detection & Auto-Sync - VOLLSTÄNDIG: BPM-Erkennung aus Audio + Deck-Synchronisation.

## Feature-Prioritätenliste (Reihenfolge)
1. ✅ Waveform Visualization - ABGESCHLOSSEN
2. ✅ Effects Processing - ABGESCHLOSSEN
3. ⏳ **Beat Detection & Auto-Sync** ← AKTUELL (VOLLSTÄNDIG implementieren)
4. ✅ MP3 Export - ABGESCHLOSSEN
5. ✅ Advanced Loop Features - ABGESCHLOSSEN
6. ⏳ Track Waveform Analysis

## Explore

### Phasen-Eintrittskriterien
- [x] Entwicklungs-Workflow gestartet

### Aufgaben

### Abgeschlossen
- [x] Bestehende Infrastruktur analysiert
- [x] BpmAnalyzer und SyncEngine geprüft
- [x] Implementierungslücken identifiziert

### Erkenntnisse
**Bestehende Infrastruktur:**
- ✅ BpmAnalyzer vorhanden (Energy-based Beat Detection)
- ✅ SyncEngine vorhanden (Master/Slave Tempo Sync)
- ✅ BPM aus Metadaten wird gelesen (TagLib)
- ✅ UI zeigt BPM an (DeckView Zeile 19)

**Was FEHLT (nicht implementiert):**
- ❌ BpmAnalyzer wird NICHT verwendet (nur Metadaten)
- ❌ Detected BPM wird NICHT an UI gesendet
- ❌ SyncEngine wird NICHT integriert
- ❌ Sync-Button in UI fehlt
- ❌ Master Deck Selection fehlt

**VOLLSTÄNDIGER SCOPE:**
- ✅ BPM-Erkennung aus Audio (BpmAnalyzer nutzen)
- ✅ Detected BPM an UI senden (IPC)
- ✅ Sync-Button in UI hinzufügen
- ✅ SyncEngine in AudioEngine integrieren
- ✅ Master Deck automatisch setzen (erster spielender Deck)
- ❌ KEINE manuelle Master-Auswahl (später)
- ❌ KEINE Phase-Sync (später)

## Plan

### Phasen-Eintrittskriterien
- [x] Anforderungen klar definiert (VOLLSTÄNDIG)
- [x] Technischer Ansatz festgelegt

### Implementierungsstrategie

**Ansatz:** BpmAnalyzer in Deck integrieren + SyncEngine in AudioEngine + Sync UI.

**Komponenten:**
1. **Deck.cs** - BpmAnalyzer während Playback nutzen, detected BPM speichern
2. **IpcProtocol** - BpmDetected Message, SetSync Message
3. **IpcServer** - Detected BPM an UI senden, Sync Commands verarbeiten
4. **AudioEngine** - SyncEngine integrieren, Tempo-Sync anwenden
5. **EngineService** - SetSyncAsync Methode, BpmDetected Event
6. **DeckViewModel** - Sync Command, detected BPM anzeigen
7. **DeckView** - Sync Button hinzufügen

**Technische Details:**
- BpmAnalyzer.AnalyzeBpm() alle 2048 Samples aufrufen
- Detected BPM via IPC an UI senden (alle 2 Sekunden)
- SyncEngine: Erster spielender Deck = Master
- Sync aktiviert → Tempo automatisch anpassen
- UI: "SYNC" Button (grün wenn aktiv)

**Zero-Break:**
- BpmAnalyzer und SyncEngine bleiben unverändert
- Nur Integration in bestehende Strukturen

### Detaillierter Implementierungsplan

#### 1. Deck.cs BPM Detection
**Datei:** `src/MIXERX.Engine/Deck.cs`
- BpmAnalyzer.AnalyzeBpm() in ProcessAudio aufrufen
- DetectedBpm Property hinzufügen

#### 2. IpcProtocol erweitern
**Datei:** `src/MIXERX.Core/IpcProtocol.cs`
- BpmDetected MessageType
- SetSync MessageType
- BpmDetectedMessage, SetSyncMessage Records

#### 3. AudioEngine Sync Integration
**Datei:** `src/MIXERX.Engine/AudioEngine.cs`
- SyncEngine Instanz
- SetSync(deckId, enabled) Methode
- Tempo-Sync in ProcessAudio anwenden

#### 4. IpcServer Updates
**Datei:** `src/MIXERX.Engine/IpcServer.cs`
- BpmDetected Message senden (periodisch)
- SetSync Handler

#### 5. EngineService
**Datei:** `src/MIXERX.UI/Services/EngineService.cs`
- SetSyncAsync(deckId, enabled)
- BpmDetected Event

#### 6. DeckViewModel
**Datei:** `src/MIXERX.UI/ViewModels/DeckViewModel.cs`
- SyncCommand
- IsSynced Property
- DetectedBpm Property (zusätzlich zu Metadata-BPM)

#### 7. DeckView UI
**Datei:** `src/MIXERX.UI/Views/DeckView.axaml`
- SYNC Button hinzufügen

### Aufgaben

### Abgeschlossen
- [x] Implementierungsstrategie definiert

## Code

### Phasen-Eintrittskriterien
- [x] Plan vollständig
- [x] Technischer Ansatz bestätigt

### Aufgaben

### Abgeschlossen
- [x] 1. Deck.cs: BpmAnalyzer bereits integriert (DetectedBpm Property vorhanden)
- [x] 2. IpcProtocol: BpmDetected, SetSync Messages hinzugefügt
- [x] 3. AudioEngine: SyncEngine integriert (SetSync, GetDetectedBpm Methoden)
- [x] 4. IpcServer: SetSync Handler hinzugefügt
- [x] 5. EngineService: SetSyncAsync Methode, BpmDetected Event
- [x] 6. DeckViewModel: Sync Command verbunden, IsSynced Property
- [x] 7. DeckView: SYNC Button hinzugefügt
- [x] 8. Build getestet (0 Errors)

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
