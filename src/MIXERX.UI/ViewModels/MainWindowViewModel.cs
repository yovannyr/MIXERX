using ReactiveUI;
using System.Reactive;
using MIXERX.Core;
using MIXERX.UI.Services;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace MIXERX.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IEngineService _engineService;
    private bool _isEngineRunning;

    public MainWindowViewModel()
    {
        _engineService = new EngineService();
        
        // Initialize deck ViewModels with real engine service
        Deck1 = new DeckViewModel(1, _engineService);
        Deck2 = new DeckViewModel(2, _engineService);
        Deck3 = new DeckViewModel(3, _engineService);
        Deck4 = new DeckViewModel(4, _engineService);
        
        Library = new LibraryViewModel(_engineService);
        Controller = new ControllerViewModel();
        
        StartEngineCommand = ReactiveCommand.CreateFromTask(StartEngine);
        StopEngineCommand = ReactiveCommand.CreateFromTask(StopEngine);

        // Menu Commands
        NewProjectCommand = ReactiveCommand.CreateFromTask(NewProject);
        OpenProjectCommand = ReactiveCommand.CreateFromTask(OpenProject);
        SaveProjectCommand = ReactiveCommand.CreateFromTask(SaveProject);
        ExitCommand = ReactiveCommand.Create(Exit);
        PreferencesCommand = ReactiveCommand.Create(ShowPreferences);
        AboutCommand = ReactiveCommand.Create(ShowAbout);
        ShowKeyboardShortcutsCommand = ReactiveCommand.Create(ShowKeyboardShortcuts);
        
        // Additional menu commands
        SaveProjectAsCommand = ReactiveCommand.CreateFromTask(SaveProjectAs);
        ImportAudioCommand = ReactiveCommand.CreateFromTask(ImportAudio);
        ExportMixCommand = ReactiveCommand.CreateFromTask(ExportMix);
        ShowLibraryCommand = ReactiveCommand.Create(ShowLibrary);
        ShowControllerMappingCommand = ReactiveCommand.Create(ShowControllerMapping);
        ShowEffectsPanelCommand = ReactiveCommand.Create(ShowEffectsPanel);
        ToggleFullScreenCommand = ReactiveCommand.Create(ToggleFullScreen);
        AudioSettingsCommand = ReactiveCommand.Create(ShowAudioSettings);
        MidiSettingsCommand = ReactiveCommand.Create(ShowMidiSettings);
        ControllerMappingCommand = ReactiveCommand.Create(ShowControllerMappingDialog);
        AnalyzeTracksCommand = ReactiveCommand.CreateFromTask(AnalyzeTracks);
        SyncLibraryCommand = ReactiveCommand.CreateFromTask(SyncLibrary);
        ShowUserManualCommand = ReactiveCommand.Create(ShowUserManual);
        
        // Recording commands
        StartRecordingCommand = ReactiveCommand.CreateFromTask(StartRecording);
        StopRecordingCommand = ReactiveCommand.CreateFromTask(StopRecording);
        
        // Auto-start engine
        _ = Task.Run(async () => await StartEngine());
    }

    public DeckViewModel Deck1 { get; }
    public DeckViewModel Deck2 { get; }
    public DeckViewModel Deck3 { get; }
    public DeckViewModel Deck4 { get; }
    public LibraryViewModel Library { get; }
    public ControllerViewModel Controller { get; }

    public bool IsEngineRunning
    {
        get => _isEngineRunning;
        set => this.RaiseAndSetIfChanged(ref _isEngineRunning, value);
    }

    public ReactiveCommand<Unit, Unit> StartEngineCommand { get; }
    public ReactiveCommand<Unit, Unit> StopEngineCommand { get; }

    // Menu Commands
    public ReactiveCommand<Unit, Unit> NewProjectCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenProjectCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveProjectCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }
    public ReactiveCommand<Unit, Unit> PreferencesCommand { get; }
    public ReactiveCommand<Unit, Unit> AboutCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowKeyboardShortcutsCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveProjectAsCommand { get; }
    public ReactiveCommand<Unit, Unit> ImportAudioCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportMixCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowLibraryCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowControllerMappingCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowEffectsPanelCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleFullScreenCommand { get; }
    public ReactiveCommand<Unit, Unit> AudioSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> MidiSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> ControllerMappingCommand { get; }
    public ReactiveCommand<Unit, Unit> AnalyzeTracksCommand { get; }
    public ReactiveCommand<Unit, Unit> SyncLibraryCommand { get; }
    public ReactiveCommand<Unit, Unit> ShowUserManualCommand { get; }
    public ReactiveCommand<Unit, Unit> StartRecordingCommand { get; }
    public ReactiveCommand<Unit, Unit> StopRecordingCommand { get; }

    private async Task StartEngine()
    {
        IsEngineRunning = await _engineService.StartEngineAsync();
    }

    private async Task StopEngine()
    {
        await _engineService.StopEngineAsync();
        IsEngineRunning = false;
    }

    private async Task NewProject()
    {
        // Reset all decks and create new project
        await Task.CompletedTask;
    }

    private async Task OpenProject()
    {
        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var topLevel = desktop?.MainWindow;
        
        if (topLevel?.StorageProvider != null)
        {
            var options = new FilePickerOpenOptions
            {
                Title = "Open MIXERX Project",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("MIXERX Project") { Patterns = ["*.mixerx"] }]
            };

            var result = await topLevel.StorageProvider.OpenFilePickerAsync(options);
            // Load project implementation
        }
    }

    private async Task SaveProject()
    {
        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var topLevel = desktop?.MainWindow;
        
        if (topLevel?.StorageProvider != null)
        {
            var options = new FilePickerSaveOptions
            {
                Title = "Save MIXERX Project",
                FileTypeChoices = [new FilePickerFileType("MIXERX Project") { Patterns = ["*.mixerx"] }]
            };

            var result = await topLevel.StorageProvider.SaveFilePickerAsync(options);
            // Save project implementation
        }
    }

    private void Exit()
    {
        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        desktop?.Shutdown();
    }

    private void ShowPreferences()
    {
        // Show preferences dialog
    }

    private void ShowAbout()
    {
        // Show about dialog
    }

    private void ShowKeyboardShortcuts()
    {
        // Show keyboard shortcuts dialog
        var shortcuts = @"MIXERX Keyboard Shortcuts:

PLAYBACK:
Space - Play/Pause Deck 1
Shift+Space - Play/Pause Deck 2

HOT CUES:
Q/W/E/R - Hot Cues 1-4 Deck 1
A/S/D/F - Hot Cues 1-4 Deck 2

LOOPS:
1 - Loop In Deck 1
2 - Loop Out Deck 1
Shift+1 - Loop In Deck 2
Shift+2 - Loop Out Deck 2

TEMPO:
← → - Nudge Tempo Deck 1
Shift+← → - Nudge Tempo Deck 2

LOADING:
Ctrl+L - Load Track to focused deck";

        System.Diagnostics.Debug.WriteLine(shortcuts);
    }
    
    private async Task SaveProjectAs()
    {
        await SaveProject();
    }
    
    private async Task ImportAudio()
    {
        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var topLevel = desktop?.MainWindow;
        
        if (topLevel?.StorageProvider != null)
        {
            var options = new FilePickerOpenOptions
            {
                Title = "Import Audio Files",
                AllowMultiple = true,
                FileTypeFilter = [new FilePickerFileType("Audio Files") 
                { 
                    Patterns = ["*.mp3", "*.flac", "*.wav", "*.aac", "*.ogg", "*.m4a"] 
                }]
            };

            await topLevel.StorageProvider.OpenFilePickerAsync(options);
        }
    }
    
    private async Task ExportMix()
    {
        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var topLevel = desktop?.MainWindow;
        
        if (topLevel?.StorageProvider != null)
        {
            var options = new FilePickerSaveOptions
            {
                Title = "Export Mix",
                FileTypeChoices = [new FilePickerFileType("Audio Files") 
                { 
                    Patterns = ["*.wav", "*.mp3"] 
                }]
            };

            await topLevel.StorageProvider.SaveFilePickerAsync(options);
        }
    }
    
    private void ShowLibrary() { }
    private void ShowControllerMapping() { }
    private void ShowEffectsPanel() { }
    private void ToggleFullScreen() { }
    private void ShowAudioSettings() { }
    private void ShowMidiSettings() { }
    private void ShowControllerMappingDialog() { }
    private async Task AnalyzeTracks() { await Task.CompletedTask; }
    private async Task SyncLibrary() { await Task.CompletedTask; }
    private void ShowUserManual() { }
    
    private async Task StartRecording()
    {
        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var topLevel = desktop?.MainWindow;
        
        if (topLevel?.StorageProvider != null)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var options = new FilePickerSaveOptions
            {
                Title = "Record Mix",
                SuggestedFileName = $"MIXERX_Recording_{timestamp}.wav",
                FileTypeChoices = [new FilePickerFileType("WAV Audio") { Patterns = ["*.wav"] }]
            };

            var result = await topLevel.StorageProvider.SaveFilePickerAsync(options);
            if (result != null)
            {
                await _engineService.StartRecordingAsync(result.Path.LocalPath);
            }
        }
    }
    
    private async Task StopRecording()
    {
        await _engineService.StopRecordingAsync();
    }
}
