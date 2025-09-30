using ReactiveUI;
using System.Reactive;
using MIXERX.Core;
using MIXERX.UI.Services;

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
        
        Library = new LibraryViewModel();
        Controller = new ControllerViewModel();
        
        StartEngineCommand = ReactiveCommand.CreateFromTask(StartEngine);
        StopEngineCommand = ReactiveCommand.CreateFromTask(StopEngine);
        
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

    private async Task StartEngine()
    {
        IsEngineRunning = await _engineService.StartEngineAsync();
    }

    private async Task StopEngine()
    {
        await _engineService.StopEngineAsync();
        IsEngineRunning = false;
    }
}
