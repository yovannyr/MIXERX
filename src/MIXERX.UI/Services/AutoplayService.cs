using System.Timers;
using MIXERX.Core.Models;
using MIXERX.UI.ViewModels;

namespace MIXERX.UI.Services;

public interface IAutoplayService
{
    bool IsEnabled { get; set; }
    void Start(LibraryViewModel library, DeckViewModel deck1, DeckViewModel deck2);
    void Stop();
    event EventHandler<Track>? NextTrackSelected;
}

public class AutoplayService : IAutoplayService
{
    private readonly System.Timers.Timer _timer;
    private LibraryViewModel? _library;
    private DeckViewModel? _deck1;
    private DeckViewModel? _deck2;
    private bool _isEnabled;
    private int _currentTrackIndex;
    private bool _activeDeck = true; // true = deck1, false = deck2

    public AutoplayService()
    {
        _timer = new System.Timers.Timer(1000); // Check every second
        _timer.Elapsed += OnTimerElapsed;
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            if (value)
                _timer.Start();
            else
                _timer.Stop();
        }
    }

    public event EventHandler<Track>? NextTrackSelected;

    public void Start(LibraryViewModel library, DeckViewModel deck1, DeckViewModel deck2)
    {
        _library = library;
        _deck1 = deck1;
        _deck2 = deck2;
        _currentTrackIndex = 0;
        IsEnabled = true;
    }

    public void Stop()
    {
        IsEnabled = false;
        _timer.Stop();
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (!IsEnabled || _library == null || _deck1 == null || _deck2 == null)
            return;

        var activeDeck = _activeDeck ? _deck1 : _deck2;
        var nextDeck = _activeDeck ? _deck2 : _deck1;

        // Check if current track is near end (last 30 seconds)
        if (IsTrackNearEnd(activeDeck))
        {
            LoadNextTrack(nextDeck);
            _activeDeck = !_activeDeck; // Switch active deck
        }
    }

    private bool IsTrackNearEnd(DeckViewModel deck)
    {
        // Simple check - in real implementation, check actual playback position
        // For now, simulate with a random condition
        return deck.IsPlaying && Random.Shared.NextDouble() < 0.01; // 1% chance per second
    }

    private void LoadNextTrack(DeckViewModel deck)
    {
        if (_library?.Tracks.Count == 0)
            return;

        _currentTrackIndex = (_currentTrackIndex + 1) % _library.Tracks.Count;
        var nextTrack = _library.Tracks[_currentTrackIndex];

        // Load track to deck
        deck.LoadTrackFromPathCommand.Execute(nextTrack.FilePath);
        
        // Notify subscribers
        NextTrackSelected?.Invoke(this, nextTrack);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
