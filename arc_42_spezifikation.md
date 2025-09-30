# Arc42 Projektspezifikation – MIXERX (.NET‑Clone von Mixxx)

> Ziel: Weltklasse‑DJ‑Software in .NET (Windows/macOS nativ), funktionsäquivalent zu Mixxx mit spürbaren Verbesserungen in Latenz, Stabilität, Bedienbarkeit, Erweiterbarkeit und Packaging.

---

## 1. Einleitung & Ziele

### 1.1 Produktvision
MIXERX ist ein leistungsstarkes, plattformübergreifendes DJ‑System für Performance, Radio‑Broadcasting und Vorbereitung (Crate‑/Playlist‑Management). Es verbindet eine moderne .NET‑Architektur mit nativen Audio‑Pfaden und einer reaktionsschnellen, GPU‑beschleunigten UI.

### 1.2 Top‑Ziele (qualitativ & messbar)
- **Ultra‑niedrige Latenz:** Round‑Trip < **10 ms** (Controller→Audio) bei 48 kHz, 128 Samples Buffer; keine hörbaren Dropouts bei CPU‑Peaks.
- **Stabilität:** 24 h Dauerbetrieb ohne XRuns/Dropouts auf Referenz‑HW; Mean‑time‑between‑failure > **100 h**.
- **Feature‑Parität+** zu Mixxx (Decks, Sync, Key/BPM‑Analyse, DVS/Vinyl, Broadcast, Recording, Hotcues/Loops, FX, Sampler, Controller‑HID/MIDI, Library etc.) plus Verbesserungen (siehe §9).
- **Benutzerführung:** Aufgaben‑Zeit (Track finden, laden, syncen, mixen) −30 % ggü. Mixxx (Usability‑Tests).
- **Packaging:** Signierte, notarized Installer (Win/MSIX; macOS .pkg/.dmg, notarized), First‑Run unter 3 s auf Referenz‑HW.

### 1.3 Stakeholder
DJs (Club/Radio/Streaming), Mapping‑Autoren, Broadcaster, Open‑Source‑Contributor, QA‑Team, Release/DevOps.

---

## 2. Randbedingungen
- **Plattformen:** Windows 10/11 x64, macOS 13+ (Intel/Apple Silicon).
- **Runtime:** .NET 9 (LTS‑Nachfolger ok), C# 12/13, optional **NativeAOT** für Engine‑/Bridge‑Prozesse.
- **Audio‑APIs:** Windows (WASAPI, ASIO*), macOS (CoreAudio). *ASIO nur bei vorhandener SDK‑Lizenz.
- **Lizenzmodell:** OSS (GPL‑kompatibel) empfohlen; Drittbibliotheken müssen kompatibel sein. Optional proprietäre Edition mit Plugin‑Bridges.
- **Security/Signing:** Windows Code Signing/EV, macOS Notarization/Entitlements, Hardened Runtime; Treiberlose HID/MIDI‑Zugriffe.
- **Internationalisierung:** 20+ Sprachen; rechts‑nach‑links Layouts.

---

## 3. Kontext & Abgrenzung

### 3.1 Fachlicher Kontext
- **Quellen:** Lokale Dateien, Netzlaufwerke; (keine DRM‑Streamingdienste).
- **Senken:** Main/Booth/Headphones, Broadcast (Icecast/Shoutcast), Recording (WAV/FLAC/MP3*). *MP3‑Encode nur, wenn rechtlich zulässig.
- **Hardware:** DJ‑Controller (MIDI & HID), Timecode‑Turntables/CDJ (DVS), Audio‑Interfaces (ASIO/CoreAudio/WASAPI).

### 3.2 Technischer Kontext
- **UI‑App** ↔ **Audio‑Engine** (separater Prozess) via SharedMemory/Ringbuffer + Protobuf‑IPC.
- **Codec‑Schicht** über FFmpeg‑Binding bzw. reine .NET‑Decoder, Recording‑Encoder.

---

## 4. Lösungsstrategie
- **Prozess‑Isolation:** Realtime‑kritische Audio‑Engine in separatem Prozess (NativeAOT möglich). UI bleibt responsiv.
- **Lock‑freie Audiopfade:** Ringpuffer, `Span<T>`/`Memory<T>`, Pinned/Pool‑Buffer; planbares Scheduling (High‑Prio Threads, MMCSS/Audio Workgroups).
- **Cross‑Platform UI:** **Avalonia UI** + **SkiaSharp/OpenGL** für GPU‑Waveforms, performante Listen/Virtualization.
- **Scripting/Mappings:** **JavaScript (Jint)** + deklarative JSON‑Mappings; Hot‑Reload, Sandbox, Typed API; Test‑Runner für Mappings.
- **Analysis‑Pipeline:** Mehrstufig (Decode→Feature‑Extraktion→Persistenz). Backpressure via `System.Threading.Channels`.
- **Konfigurierbare Engines:** Wählbare Time‑Stretch/Pitch Engines (RubberBand, SoundTouch, ggf. Dirac*), Key/BPM Engines (libkeyfinder‑Wrapper, Aubio.NET, NWaves‑basierte Verfahren).
- **DVS:** Zeitcode‑Demodulation (xwax‑Algorithmus portiert/wrapped), robustes Tracking (Kalman‑Filter), Noise‑Gate, RIAA‑EQ.
- **Qualitätssicherung:** Property‑based Tests, Golden‑Master‑Audio, Profiling (ETW/Perfetto), Latenzmessungen (Loopback, Hardware‑Timer).

---

## 5. Bausteinsicht (Hauptbausteine)

1. **Shell/UI** (Avalonia): Views, Commands, Shortcuts, Touch/HiDPI, Themen, A11y.  
2. **Controller‑Manager:** MIDI/HID Discovery, Mapping‑Engine (JS/JSON), Feedback (LED/Displays), Gerätekonfigurationen.  
3. **Decks & Transport:** Deck‑State‑Machine, Caching‑Reader, Beatgrid, Sync‑/Tempo‑Master, Quantize, Slip, Reverse.  
4. **Audio‑Engine (RT):** Graph (Player→Timestretch→EQ/FX→Mixer→Out); Resampler; Mixer (float32/float64); Headroom/Soft‑Clip; Monitor Bus.  
5. **FX‑Host:** In‑App‑FX (EQ, Filter, Echo, Reverb, Flanger, Phaser, Compressor, Limiter), Plugin‑Bridges (optional VST3 host).  
6. **Analysis:** Decoder‑Abstraktionen, BPM/Onset, Key/Chroma, ReplayGain/LUFS, Waveform‑Tiles, Fingerprint.  
7. **DVS/VinylControl:** Timecode‑Demod, Deck‑Control, Needle‑Drop, Sticker‑Lock, Speed/Pitch‑Tracking.  
8. **Library:** Scan/Watch, Tagging, Suche/Filter, Crates/Playlists, Smart‑Crates, Historie, externes Import/Export (Traktor/Serato/Rekordbox).  
9. **Streaming & Recording:** Icecast/Shoutcast Source, Metadata, Source‑Mix; Recording (WAV/FLAC/MP3*), Split by Cue/Track.  
10. **Persistence:** SQLite‑DB, Migrations, Backup/Restore; Blob‑Cache (Waveform Tiles).  
11. **IPC/Contracts:** Protobuf Nachrichten, Versionierung, Health/Watchdog.  
12. **Telemetry & Logging:** strukturiert (ETW/EventPipe), Session‑Dumps, Crash‑Reports (opt‑in).

---

## 6. Laufzeitsichten (Beispiele)

### 6.1 „Track Laden & Sync“
1) UI lädt Track → Library liefert Pfad + Analyse‑Status → Decoder Prefetch → Deck State READY.  
2) Sync an: BPM‑/Beatgrid‑Align → Phase‑Lock → Tempo‑Fader Anpassung.  
3) Start: Transport→Engine Graph→Mixer→Output; Controller‑Feedback (LED/Screen) via Mapping.

### 6.2 „DVS‑Scratch“
Input (Phono/Line) → RIAA‑EQ → Bandpass → Timecode‑Demod → Pitch/Phase Filter (Kalman) → Deck Transport (Scratch) → Mixer → Output.

### 6.3 „Broadcast“
Master Bus → Encoder (Opus/MP3/Ogg) → Icecast Sink; Metadata Updates; Network Backoff/Retry; Peak/RMS/LUFS Metering.

---

## 7. Verteilung/Deployment
- **Prozesse:** `Mixerx.UI` (App), `Mixerx.Engine` (RT), optional `Mixerx.PluginHost` (Sandbox).
- **Pakete:** Win: MSIX + VC‑Redist bundled; macOS: Universal Binary (x64/ARM64), Hardened Runtime & Notarization.
- **Assets‑Cache:** `%APPDATA%/MIXERX` bzw. `~/Library/Application Support/MIXERX` (DB, Waveforms, Logs).

---

## 8. Querschnittliche Konzepte
- **Zeit/Clocking:** Monotonic Clock, Audio Clock, Host Sync (Ableton Link* optional), Phasen‑Lock.
- **Fehler‑Robustheit:** XRuns‑Erkennung, Engine‑Watchdog, Auto‑Recovery; Safe‑State beim Plugin‑Crash.
- **Performance:** SustainedLowLatency‑GC in UI, kein GC im RT‑Pfad; vor‑alloziertes Memory; SIMD/Intrinsics; GPU‑Batching.
- **Konfiguration:** Staging/Presets, per‑Device Overrides, Export/Import.
- **Testbarkeit:** Headless Engine, deterministische Seeds, Golden‑Master WAVs, Latenz‑Smoke‑Tests in CI.
- **Barrierefreiheit:** Screenreader‑Labels, hohe Kontraste, skalierbare UI, Tastatur‑Only Workflows.

---

## 9. Architekturentscheidungen & Verbesserungen ggü. Mixxx

### 9.1 Schlüsselige Entscheidungen
- **UI:** *Avalonia UI* statt Qt → einheitliche .NET‑Codebase, moderne MVVM, schnelle Desktop‑Bereitstellung.  
- **Audio‑Backend:** Primär **miniaudio.NET** (WASAPI/CoreAudio/ASIO via Backend), Windows‑Fallback **NAudio**; macOS P/Invoke‑Pfad zu CoreAudio bei Bedarf.  
- **IPC:** **Google.Protobuf / protobuf‑net** für effizienten Engine‑IPC.  
- **FX & Plugins:** In‑App‑FX first; optional VST3‑Host (separater, isolierter Prozess).  
- **Scripting:** **Jint** (ECMAScript) statt QtScript/QJSEngine; Hot‑Reload, Debug‑API, Tests.  
- **Rendering:** **SkiaSharp/OpenGL** für Waveforms/Spectrogramme; tile‑basiert, GPU‑instanziert.

### 9.2 Verbesserungen (kritische Prüfung bekannter Schwächen)
- **Abhängigkeits‑Hölle (z. B. Protobuf‑Versionen):** Engine/Codec kapseln; stabile NuGet‑Versionierung; Trimming/AOT‑Analyse; reproduzierbare Builds.
- **Mapping‑Komplexität:** Typisierte, versionierte Mapping‑API; GUI‑Mapper; CI‑Tests für Mappings; Beispiel‑Kits.
- **DVS‑Stabilität:** Robustere Filterung (Kalman), dynamische Jitter‑Anpassung, Sticker‑Lock‑Kalibrierung, bessere Noise‑Rejektion.
- **Waveform‑Performance:** Tile‑Caching + GPU Batching; Precompute auf Idle; adaptive Detailtiefe.
- **Library‑Skalierung:** Async‑Scanner, Debounce‑FS‑Watcher, Bulk‑Tagging, Schnellsuche (FTS5).
- **Broadcast‑Resilienz:** Retry/Backoff, geringere Reconnect‑Gaps, Metadata‑Queue, Netz‑Jitter‑Puffer.
- **Packaging:** Signierte, notarized Installer; Systemcheck/Diagnose‑Berichte; Treiber‑Kompass (ASIO/WASAPI/CoreAudio).

---

## 10. Qualitätsanforderungen (Q‑Szenarien)

| Qualität | Szenario | Metrik/Ziel |
|---|---|---|
| Latenz | Jog‑Wheel zu Audio | ≤10 ms Round‑Trip @48 kHz/128 Samples |
| Stabilität | 24 h Live‑Set | 0 Engine‑Crashes, 0 Dropouts |
| Performance | 4 Decks + 2 Sampler + FX | <60 % CPU auf Referenz (i7/8‑Kern, M2) |
| Startzeit | Kaltstart | <3 s bis UI bereit |
| Library | Scan 50k Tracks | ≥500 Tracks/min Analyse (ohne Key/BPM) |
| Usability | 5 Kernaufgaben | −30 % Zeit ggü. Benchmark |
| Sicherheit | Crash‑Recovery | Kein Datenverlust; Auto‑Restore in <10 s |

---

## 11. Risiken & Gegenmaßnahmen

- **GC‑Pauses im RT‑Pfad:** RT‑Pfad unmanaged/NativeAOT; keine Allokationen; Pinned Pools; Engine in eigenem Prozess.
- **ASIO‑Lizenz/Support:** Optionale Komponente; klare Nutzerführung; Fallback WASAPI Exclusive.
- **Codec‑Rechtelage:** MP3‑Encode optional, Hinweisdialoge; bevorzugt offene Formate (Opus/FLAC).
- **VST‑Hosting‑Stabilität:** Strikte Prozess‑Isolation, Life‑Cycle‑Supervisor, Notbremse.
- **Heterogene Controller:** HID Parser + Matching‑Heuristik; Community‑Mappings; Auto‑Update‑Feed.

---

## 12. Baustein‑Schnittstellen (Auszug)

- **Audio Engine API:** `Start/Stop`, `LoadTrack(uri)`, `SetTempo/Key/Pitch`, `SetLoop/Hotcue`, `GetMeters`, `SubscribeEvents`.
- **Mapping API (JS):** `onMidi(in)`, `sendMidi(out)`, `deck[i].play()`, `mixer.setGain(db)`, `screen.write(text)`, `led.set(state)`.
- **Library API:** `Import(path)`, `Analyze(ids, options)`, `Search(query)`, `Export(format)`.
- **Broadcast API:** `Connect(url, creds)`, `SetMetadata`, `Start/Stop`, `Status`.

---

## 13. Roadmap & Arbeitspakete

**M0 – Architektur‑Skeleton (4–6 Wo):** Prozesse, IPC, leere Engine, CI, Crash‑Handling.  
**M1 – Audio I/O & Playback (6–8 Wo):** Decoder‑Abstraktion, 2 Decks, Mixer, Recording (WAV), Metering.  
**M2 – Analysis (4–6 Wo):** BPM/Beatgrid, Waveform‑Tiles, ReplayGain, DB‑Persistenz.  
**M3 – UI & Controller (6–8 Wo):** Deck‑UI, Browser, Jint‑Mappings, 1–2 Referenz‑Controller.  
**M4 – FX & Timestretch (6–8 Wo):** EQ/Filter/Delay/Reverb, RubberBand/SoundTouch Auswahl.  
**M5 – DVS (6–8 Wo):** Timecode‑Demod, Sticker‑Lock, Kalibrierung.  
**M6 – Broadcast & Formate (4–6 Wo):** Icecast/Shoutcast, FLAC/Opus/MP3*, Metadata.  
**M7 – Polishing & Release (6 Wo):** A11y/i18n, Installer, Notarization, Telemetrie‑Opt‑in.

---

## 14. Technologie‑/Bibliotheksmatrix (C/C++ → .NET Alternativen)

> *Primäre Vorschläge, Alternativen und Hinweise; Plattform‑Eignung und Lizenz prüfen.*

| Kategorie (C/C++) | In Mixxx üblich | .NET/Managed Alternative | Anmerkungen |
|---|---|---|---|
| GUI/Framework | Qt | **Avalonia UI**, ggf. .NET MAUI | Avalonia für Desktop (Win/macOS) sehr geeignet. |
| Audio I/O | PortAudio | **miniaudio.NET**, **NAudio** (Win), CoreAudio P/Invoke | Miniaudio deckt Win/macOS; ASIO optional. |
| MIDI | PortMIDI | **DryWetMIDI**, RtMidiSharp | DryWetMIDI aktiv gepflegt. |
| HID/Controller | HID api | **HidSharp**, Hid.Net | Cross‑Platform HID Zugriff. |
| Time‑Stretch/Pitch | Rubber Band, SoundTouch | **RubberBandSharp** (Wrapper), **SoundTouch.Net** | RubberBand liefert Qualität; Fallback SoundTouch. |
| Decoder/Codecs | FFmpeg, libmad, libvorbis, opusfile, flac | **FFmpeg.AutoGen/FFMpegCore**, **NVorbis**, **Concentus.Opus**, **FlacBox** | Für maximale Breite FFmpeg‑Binding. |
| Tagging | TagLib | **TagLib# (taglib‑sharp)** | Stabil, breit genutzt. |
| Fingerprint | Chromaprint | **Chromaprint.NET / AcoustID‑Clients** | Fingerprint/Lookup Unterstützung. |
| Key Detection | libkeyfinder | **Wrapper (P/Invoke)**, **NWaves‑basierte Implementierung**, **aubio.net** | Start mit Wrapper, später managed Algorithmus. |
| BPM/Onset | SoundTouch (BPMDetect) | **NWaves**, **aubio.net** | Kombi aus Onset+Tempo Tracking. |
| FFT/NumPy | FFTW/Eigen | **Math.NET Numerics**, **FftSharp** | FFT/Filter/Linear Algebra. |
| DB | SQLite | **Microsoft.Data.Sqlite** (+EF Core/Dapper) | FTS5/Indexes für Suche. |
| Streaming | libshout | **LibShout‑csharp**, eigener Icecast‑Client | Getrennte Source‑Clients. |
| Scripting | QtScript/QJSEngine | **Jint**, ClearScript (V8) | Jint = pure managed, sicher, portabel. |
| Plugin‑Host | LADSPA/LV2 | **Eigene FX + optional VST3‑Host (AudioPlugSharp/NPlug)** | Strikte Prozess‑Isolation. |

---

## 15. Datenmodell (vereinfacht)
- **Track**(Id, Path, Tags, BPM, Key, ReplayGain, Duration, WaveformId, Fingerprint, …)
- **DeckState**(DeckId, TrackId, Pos, Tempo, Pitch, KeyShift, Loop, Hotcues[], Slip, …)
- **AnalysisJob**(Id, TrackId, Stages, Status, Error)
- **ControllerProfile**(Vendor, Product, Mapping, Firmware, Capabilities)
- **BroadcastProfile**(Url, Format, Bitrate, MetadataMode)

---

## 16. Sicherheit & Datenschutz
- Keine stille Telemetrie; **Opt‑In** für anonyme Stabilitätsdaten.
- Keine Cloud‑Bindung.
- Signierte Updates, verifizierte Mapping‑Pakete.

---

## 17. Internationalisierung & A11y
- Lokalisierung via resx/PO; Live‑Reload.
- Screenreader‑Navigationsstruktur, Fokusreihenfolge, Shortcut‑Matrix, Farbkontraste ≥ WCAG AA.

---

## 18. Test/QA‑Strategie
- **Unit:** DSP (Deterministik), Parser, DB.
- **Property‑based:** Parser/Codecs/Mapping.
- **Integration:** Engine‑Pipelines, DVS‑E2E mit Referenz‑Timecodes.
- **System:** Latenz/Dropout/Glitch‑Tests, 24 h Soak, Broadcast‑Reconnect, Controller‑Hotplug.

---

## 19. Migrations-/Importpfade
- Import: Mixxx DB, Traktor NML, Serato Crates, Rekordbox XML/DB (soweit rechtlich/technisch möglich).
- Mapping‑Konverter (Mixxx JS → Jint/JSON).

---

## 20. Glossar
- **DVS:** Digital Vinyl System.
- **XRuns:** Buffer‑Underruns/Overruns.
- **AOT:** Ahead‑of‑Time Compilation.
- **FTS:** Full‑Text‑Search.

---

## 21. Überprüfung (Checkliste)
- Ziele quantifiziert & testbar? ✔️  
- Realtime‑Pfad ohne GC/Allokationen? ✔️ (eigener Prozess, Pinned Pools)  
- Plattform‑APIs abgedeckt (WASAPI/ASIO/CoreAudio)? ✔️  
- UI performant & barrierefrei? ✔️ (Avalonia+Skia, A11y)  
- Packaging & Signing geplant? ✔️  
- Bibliotheksalternativen dokumentiert? ✔️  
- Risiken adressiert? ✔️

---

## 22. Anhang A – Beispiel‑Konfigs
- **Audio:** 48 kHz, 128/256 Samples, WASAPI Exclusive oder CoreAudio.
- **Engine:** Float32, 6 dB Headroom, Soft‑Limiter optional.
- **Analysis:** BPM „Percussive“, Key „Mixed (Chroma+HPCP)“.

## 23. Anhang B – Mapping‑API (Ausschnitt)
```js
export function onMidi(msg) {
  const d = deck(1);
  if (msg.isNoteOn(0x10)) d.playPause();
  if (msg.isCC(0x20)) d.jog(msg.delta());
  if (msg.isCC(0x30)) mixer.gain.setDb(ccToDb(msg.value));
}
```

## 24. Anhang C – Telemetrie‑Ereignisse (Opt‑In)
- `engine.drop`(buffer, durationMs, device)  
- `broadcast.disconnect`(code)  
- `controller.hotplug`(vid/pid)

