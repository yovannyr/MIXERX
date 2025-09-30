using ReactiveUI;
using System.Reactive;
using MIXERX.Core;
using MIXERX.UI.Services;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Media.Imaging;
using TagLib;

namespace MIXERX.UI.ViewModels
{
    // ViewModel for a single Hot Cue
    public class HotCueViewModel : ReactiveObject
    {
        private bool _isSet;
        private object? _color; // Use object for now, replace with HotCueColor if available
        private string _label = string.Empty;

        public bool IsSet
        {
            get => _isSet;
            set => this.RaiseAndSetIfChanged(ref _isSet, value);
        }

        public object? Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        public string Label
        {
            get => _label;
            set => this.RaiseAndSetIfChanged(ref _label, value);
        }
    }

    public class DeckViewModel : ViewModelBase
    {
    // Hot Cue properties for binding in DeckView.axaml
    public HotCueViewModel HotCue1 { get; } = new HotCueViewModel();
    public HotCueViewModel HotCue2 { get; } = new HotCueViewModel();
    public HotCueViewModel HotCue3 { get; } = new HotCueViewModel();
    public HotCueViewModel HotCue4 { get; } = new HotCueViewModel();
    public HotCueViewModel HotCue5 { get; } = new HotCueViewModel();
    public HotCueViewModel HotCue6 { get; } = new HotCueViewModel();
    public HotCueViewModel HotCue7 { get; } = new HotCueViewModel();
    public HotCueViewModel HotCue8 { get; } = new HotCueViewModel();
    private readonly int _deckId;
    private readonly IEngineService _engineService;
    private string _trackName = "No Track Loaded";
    private bool _isPlaying;
    private double _tempo = 1.0;
    private double _position;
    
    // Effects controls
    private double _eqLow = 1.0;
    private double _eqMid = 1.0;
    private double _eqHigh = 1.0;
    private double _filterCutoff = 1.0;
    private double _filterResonance = 0.1;
    
    // Visual properties
    private float[]? _waveformData;
    private double _volume = 1.0;
    private double _vinylRpm = 33.3;
    private Bitmap? _albumCover;
    
    // Track info
    private string _bpm = "0.0";
    private string _key = "";
    
    // UI Display properties
    public string DeckNumber => $"DECK {_deckId}";
    public string PlayButtonText => IsPlaying ? "⏸" : "▶";
    public string PlayButtonColor => IsPlaying ? "#ff4444" : "#44ff44";
    public string TempoDisplay => $"{(_tempo - 1) * 100:+0;-0}%";

    public float[]? WaveformData
    {
        get => _waveformData;
        set => this.RaiseAndSetIfChanged(ref _waveformData, value);
    }

    public double Volume
    {
        get => _volume;
        set => this.RaiseAndSetIfChanged(ref _volume, value);
    }

    public double VinylRpm
    {
        get => _vinylRpm;
        set => this.RaiseAndSetIfChanged(ref _vinylRpm, value);
    }

    public Bitmap? AlbumCover
    {
        get => _albumCover;
        set => this.RaiseAndSetIfChanged(ref _albumCover, value);
    }

    public DeckViewModel(int deckId, IEngineService engineService)
    {
        _deckId = deckId;
        _engineService = engineService;
        
        PlayPauseCommand = ReactiveCommand.CreateFromTask(PlayPause);
        LoadTrackCommand = ReactiveCommand.CreateFromTask(LoadTrack);
        SyncCommand = ReactiveCommand.CreateFromTask(Sync);
        SetCueCommand = ReactiveCommand.CreateFromTask(SetCue);
        
        // Loop commands
        AutoLoopCommand = ReactiveCommand.CreateFromTask<object>(param =>
        {
            var beats = Convert.ToInt32(param);
            return SetAutoLoop(beats);
        });
        LoopInCommand = ReactiveCommand.CreateFromTask(SetLoopIn);
        LoopOutCommand = ReactiveCommand.CreateFromTask(SetLoopOut);
        ExitLoopCommand = ReactiveCommand.CreateFromTask(ExitLoop);
    }

    public string TrackName
    {
        get => _trackName;
        set => this.RaiseAndSetIfChanged(ref _trackName, value);
    }

    public bool IsPlaying
    {
        get => _isPlaying;
        set => this.RaiseAndSetIfChanged(ref _isPlaying, value);
    }

    public double Tempo
    {
        get => _tempo;
        set
        {
            this.RaiseAndSetIfChanged(ref _tempo, value);
            _ = _engineService.SetTempoAsync(_deckId, (float)value);
        }
    }

    public double Position
    {
        get => _position;
        set
        {
            this.RaiseAndSetIfChanged(ref _position, value);
            _ = _engineService.SetPositionAsync(_deckId, (float)value);
        }
    }

    // EQ Controls
    public double EqLow
    {
        get => _eqLow;
        set
        {
            this.RaiseAndSetIfChanged(ref _eqLow, value);
            _ = _engineService.SetEffectParameterAsync(_deckId, "EQ", "low", (float)value);
        }
    }

    public double EqMid
    {
        get => _eqMid;
        set
        {
            this.RaiseAndSetIfChanged(ref _eqMid, value);
            _ = _engineService.SetEffectParameterAsync(_deckId, "EQ", "mid", (float)value);
        }
    }

    public double EqHigh
    {
        get => _eqHigh;
        set
        {
            this.RaiseAndSetIfChanged(ref _eqHigh, value);
            _ = _engineService.SetEffectParameterAsync(_deckId, "EQ", "high", (float)value);
        }
    }

    // Filter Controls
    public double FilterCutoff
    {
        get => _filterCutoff;
        set
        {
            this.RaiseAndSetIfChanged(ref _filterCutoff, value);
            _ = _engineService.SetEffectParameterAsync(_deckId, "Filter", "cutoff", (float)value);
        }
    }

    public double FilterResonance
    {
        get => _filterResonance;
        set
        {
            this.RaiseAndSetIfChanged(ref _filterResonance, value);
            _ = _engineService.SetEffectParameterAsync(_deckId, "Filter", "resonance", (float)value);
        }
    }

    // Track Info
    public string Bpm
    {
        get => _bpm;
        set => this.RaiseAndSetIfChanged(ref _bpm, value);
    }

    public string Key
    {
        get => _key;
        set => this.RaiseAndSetIfChanged(ref _key, value);
    }

    // Loop properties
    private bool _isLooping;
    private int _loopLengthBeats;
    private float _loopProgress;

    public bool IsLooping
    {
        get => _isLooping;
        set => this.RaiseAndSetIfChanged(ref _isLooping, value);
    }

    public int LoopLengthBeats
    {
        get => _loopLengthBeats;
        set => this.RaiseAndSetIfChanged(ref _loopLengthBeats, value);
    }

    public float LoopProgress
    {
        get => _loopProgress;
        set => this.RaiseAndSetIfChanged(ref _loopProgress, value);
    }

    public ReactiveCommand<Unit, Unit> PlayPauseCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadTrackCommand { get; }
    public ReactiveCommand<Unit, Unit> SyncCommand { get; }
    public ReactiveCommand<Unit, Unit> SetCueCommand { get; }
    
    // Loop commands
    public ReactiveCommand<object, Unit> AutoLoopCommand { get; }
    public ReactiveCommand<Unit, Unit> LoopInCommand { get; }
    public ReactiveCommand<Unit, Unit> LoopOutCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitLoopCommand { get; }

    private async Task PlayPause()
    {
        if (IsPlaying)
        {
            await _engineService.PauseAsync(_deckId);
            IsPlaying = false;
        }
        else
        {
            await _engineService.PlayAsync(_deckId);
            IsPlaying = true;
        }
    }

    private async Task LoadTrack()
    {
        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var topLevel = desktop?.MainWindow;
        
        if (topLevel?.StorageProvider != null)
        {
            var options = new FilePickerOpenOptions
            {
                Title = $"Load Track for Deck {_deckId}",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("Audio Files") { Patterns = ["*.mp3", "*.wav", "*.flac", "*.ogg", "*.m4a"] }]
            };

            var result = await topLevel.StorageProvider.OpenFilePickerAsync(options);
            var filePath = result.FirstOrDefault()?.Path.LocalPath;

            if (!string.IsNullOrEmpty(filePath))
            {
                await _engineService.LoadTrackAsync(_deckId, filePath);
                TrackName = Path.GetFileNameWithoutExtension(filePath);
                
                // Extract album cover and generate waveform
                await ExtractAlbumCoverAsync(filePath);
                await GenerateWaveformAsync(filePath);
                
                // Start periodic status updates to get BPM/Key
                _ = Task.Run(UpdateTrackInfo);
            }
        }
    }

    private Task SetCue()
    {
        // Set cue point at current position
        // await _engineService.SetCuePointAsync(_deckId, Position);
        System.Diagnostics.Debug.WriteLine($"Cue point set at {Position:F2} on Deck {_deckId}");
        return Task.CompletedTask;
    }

    private Task SetAutoLoop(int beats)
    {
        // Send auto-loop command to engine
        // await _engineService.SetAutoLoopAsync(_deckId, beats);
        
        // Update UI state
        IsLooping = true;
        LoopLengthBeats = beats;
        System.Diagnostics.Debug.WriteLine($"Auto-Loop {beats} beats on Deck {_deckId}");
        return Task.CompletedTask;
    }

    private Task SetLoopIn()
    {
        // await _engineService.SetLoopInAsync(_deckId);
        System.Diagnostics.Debug.WriteLine($"Loop In set on Deck {_deckId}");
        return Task.CompletedTask;
    }

    private Task SetLoopOut()
    {
        // await _engineService.SetLoopOutAsync(_deckId);
        IsLooping = true;
        System.Diagnostics.Debug.WriteLine($"Loop Out set on Deck {_deckId}");
        return Task.CompletedTask;
    }

    private Task ExitLoop()
    {
        // await _engineService.ExitLoopAsync(_deckId);
        IsLooping = false;
        LoopProgress = 0;
        System.Diagnostics.Debug.WriteLine($"Exit Loop on Deck {_deckId}");
        return Task.CompletedTask;
    }

    private Task Sync()
    {
        // Sync this deck to master deck
        // Implementation would depend on sync service
        System.Diagnostics.Debug.WriteLine($"Sync requested for Deck {_deckId}");
        return Task.CompletedTask;
    }

    private async Task ExtractAlbumCoverAsync(string filePath)
    {
        try
        {
            await Task.Run(() =>
            {
                using var file = TagLib.File.Create(filePath);
                var pictures = file.Tag.Pictures;
                
                if (pictures.Length > 0)
                {
                    var picture = pictures[0];
                    using var stream = new MemoryStream(picture.Data.Data);
                    
                    // Create bitmap from album cover data
                    AlbumCover = new Bitmap(stream);
                    
                    // Also extract metadata
                    Bpm = file.Tag.BeatsPerMinute > 0 ? file.Tag.BeatsPerMinute.ToString() : "0.0";
                    Key = !string.IsNullOrEmpty(file.Tag.InitialKey) ? file.Tag.InitialKey : "";
                }
                else
                {
                    // No album cover found
                    AlbumCover = null;
                }
            });
        }
        catch
        {
            // Fallback: no album cover
            AlbumCover = null;
        }
    }

    private async Task GenerateWaveformAsync(string filePath)
    {
        try
        {
            // Simple waveform generation (in production, use proper audio analysis)
            await Task.Run(() =>
            {
                var waveform = new float[1000]; // 1000 points for visualization
                var random = new Random();
                
                // Generate mock waveform data
                for (int i = 0; i < waveform.Length; i++)
                {
                    // Simulate audio waveform with some randomness
                    var baseAmplitude = (float)Math.Sin(i * 0.1) * 0.5f;
                    var noise = (float)(random.NextDouble() - 0.5) * 0.3f;
                    waveform[i] = Math.Clamp(baseAmplitude + noise, -1.0f, 1.0f);
                }
                
                WaveformData = waveform;
            });
        }
        catch
        {
            // Fallback to empty waveform
            WaveformData = new float[1000];
        }
    }

    private async Task UpdateTrackInfo()
    {
        // Periodically update track info (BPM, Key) from engine
        while (!string.IsNullOrEmpty(TrackName))
        {
            try
            {
                var status = await _engineService.GetStatusAsync(_deckId);
                if (status != null)
                {
                    // Update UI with detected BPM/Key
                    // Bpm = status.DetectedBpm.ToString("F1");
                    // Key = status.DetectedKey;
                }

                // Update vinyl RPM based on tempo
                VinylRpm = 33.3 * Tempo;
                
                // Simulate loop progress (in real implementation, get from engine)
                if (IsLooping)
                {
                    LoopProgress = (float)((DateTime.Now.Millisecond / 1000.0) % 1.0);
                }
                
                // Trigger UI updates for animation
                this.RaisePropertyChanged(nameof(PlayButtonText));
                this.RaisePropertyChanged(nameof(PlayButtonColor));
                this.RaisePropertyChanged(nameof(TempoDisplay));
            }
            catch
            {
                // Ignore errors
            }
            
            await Task.Delay(100); // Update 10 times per second for smooth animation
        }
    }
    }
}
