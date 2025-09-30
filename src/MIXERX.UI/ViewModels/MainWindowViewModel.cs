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
}
