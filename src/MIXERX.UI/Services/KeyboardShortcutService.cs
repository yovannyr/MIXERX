using Avalonia.Input;
using System.Collections.Generic;

namespace MIXERX.UI.Services
{
    public interface IKeyboardShortcutService
    {
        void RegisterShortcuts();
        bool IsShortcutRegistered(string key);
        void HandleKeyPress(Key key, KeyModifiers modifiers);
    }

    public class KeyboardShortcutService : IKeyboardShortcutService
    {
        private readonly Dictionary<string, Action> _shortcuts = new();
        private readonly IEngineService _engineService;

        public KeyboardShortcutService(IEngineService? engineService = null)
        {
            _engineService = engineService ?? new EngineService();
            RegisterShortcuts();
        }

        public void RegisterShortcuts()
        {
            // PLAYBACK - Serato compatible (W=Left deck, S=Right deck)
            _shortcuts["W"] = () => _ = _engineService.PlayAsync(1);
            _shortcuts["S"] = () => _ = _engineService.PlayAsync(2);
            _shortcuts["Q"] = () => ReversePlay(1);
            _shortcuts["A"] = () => ReversePlay(2);
            
            // TRACK LOADING
            _shortcuts["Shift+Left"] = () => LoadSelectedTrack(1);
            _shortcuts["Shift+Right"] = () => LoadSelectedTrack(2);
            _shortcuts["Alt+W"] = () => LoadNextTrack(1);
            _shortcuts["Alt+S"] = () => LoadNextTrack(2);
            _shortcuts["Alt+Q"] = () => LoadPrevTrack(1);
            _shortcuts["Alt+A"] = () => LoadPrevTrack(2);
            
            // CUE POINTS - Left Deck (1-5), Right Deck (6-0)
            _shortcuts["1"] = () => TriggerHotCue(1, 1);
            _shortcuts["2"] = () => TriggerHotCue(1, 2);
            _shortcuts["3"] = () => TriggerHotCue(1, 3);
            _shortcuts["4"] = () => TriggerHotCue(1, 4);
            _shortcuts["5"] = () => TriggerHotCue(1, 5);
            _shortcuts["6"] = () => TriggerHotCue(2, 1);
            _shortcuts["7"] = () => TriggerHotCue(2, 2);
            _shortcuts["8"] = () => TriggerHotCue(2, 3);
            _shortcuts["9"] = () => TriggerHotCue(2, 4);
            _shortcuts["0"] = () => TriggerHotCue(2, 5);
            
            // PITCH BEND
            _shortcuts["T"] = () => PitchBend(1, -0.02f);
            _shortcuts["Y"] = () => PitchBend(1, 0.02f);
            _shortcuts["G"] = () => PitchBend(2, -0.02f);
            _shortcuts["H"] = () => PitchBend(2, 0.02f);
            
            // PITCH CONTROL
            _shortcuts["E"] = () => AdjustPitch(1, -0.01f);
            _shortcuts["R"] = () => AdjustPitch(1, 0.01f);
            _shortcuts["D"] = () => AdjustPitch(2, -0.01f);
            _shortcuts["F"] = () => AdjustPitch(2, 0.01f);
            
            // LOOP CONTROLS
            _shortcuts["OemOpenBrackets"] = () => ToggleLoop(1);
            _shortcuts["OemQuotes"] = () => ToggleLoop(2);
            _shortcuts["O"] = () => SetLoopIn(1);
            _shortcuts["P"] = () => SetLoopOut(1);
            _shortcuts["K"] = () => SetLoopIn(2);
            _shortcuts["L"] = () => SetLoopOut(2);
            
            // LIBRARY NAVIGATION
            _shortcuts["Up"] = () => LibraryUp();
            _shortcuts["Down"] = () => LibraryDown();
            _shortcuts["Ctrl+F"] = () => FocusSearch();
            
            // KEYLOCK
            _shortcuts["F5"] = () => ToggleKeylock(1);
            _shortcuts["F10"] = () => ToggleKeylock(2);
            
            // CENSOR
            _shortcuts["U"] = () => ToggleCensor(1);
            _shortcuts["J"] = () => ToggleCensor(2);
        }

        public bool IsShortcutRegistered(string key)
        {
            return _shortcuts.ContainsKey(key);
        }

        public void HandleKeyPress(Key key, KeyModifiers modifiers)
        {
            var keyString = BuildKeyString(key, modifiers);
            if (_shortcuts.TryGetValue(keyString, out var action))
            {
                action.Invoke();
            }
        }

        private string BuildKeyString(Key key, KeyModifiers modifiers)
        {
            var result = "";
            if (modifiers.HasFlag(KeyModifiers.Shift)) result += "Shift+";
            if (modifiers.HasFlag(KeyModifiers.Control)) result += "Ctrl+";
            if (modifiers.HasFlag(KeyModifiers.Alt)) result += "Alt+";
            
            result += key.ToString();
            return result;
        }

        private void TriggerHotCue(int deckId, int cueNumber)
        {
            System.Diagnostics.Debug.WriteLine($"Hot Cue {cueNumber} triggered on Deck {deckId}");
        }

        private void SetLoopIn(int deckId)
        {
            System.Diagnostics.Debug.WriteLine($"Loop In set on Deck {deckId}");
        }

        private void SetLoopOut(int deckId)
        {
            System.Diagnostics.Debug.WriteLine($"Loop Out set on Deck {deckId}");
        }

        private void NudgeTempo(int deckId, float delta)
        {
            System.Diagnostics.Debug.WriteLine($"Tempo nudge {delta:+0.00;-0.00} on Deck {deckId}");
        }

        private void LoadTrackToFocusedDeck()
        {
            System.Diagnostics.Debug.WriteLine("Load track to focused deck");
        }

        private void ReversePlay(int deckId)
        {
            System.Diagnostics.Debug.WriteLine($"Reverse play on Deck {deckId}");
        }

        private void LoadSelectedTrack(int deckId)
        {
            System.Diagnostics.Debug.WriteLine($"Load selected track to Deck {deckId}");
        }

        private void LoadNextTrack(int deckId)
        {
            System.Diagnostics.Debug.WriteLine($"Load next track to Deck {deckId}");
        }

        private void LoadPrevTrack(int deckId)
        {
            System.Diagnostics.Debug.WriteLine($"Load previous track to Deck {deckId}");
        }

        private void PitchBend(int deckId, float delta)
        {
            System.Diagnostics.Debug.WriteLine($"Pitch bend {delta:+0.00;-0.00} on Deck {deckId}");
        }

        private void AdjustPitch(int deckId, float delta)
        {
            System.Diagnostics.Debug.WriteLine($"Adjust pitch {delta:+0.00;-0.00} on Deck {deckId}");
        }

        private void ToggleLoop(int deckId)
        {
            System.Diagnostics.Debug.WriteLine($"Toggle loop on Deck {deckId}");
        }

        private void LibraryUp()
        {
            System.Diagnostics.Debug.WriteLine("Library navigation up");
        }

        private void LibraryDown()
        {
            System.Diagnostics.Debug.WriteLine("Library navigation down");
        }

        private void FocusSearch()
        {
            System.Diagnostics.Debug.WriteLine("Focus library search");
        }

        private void ToggleKeylock(int deckId)
        {
            System.Diagnostics.Debug.WriteLine($"Toggle keylock on Deck {deckId}");
        }

        private void ToggleCensor(int deckId)
        {
            System.Diagnostics.Debug.WriteLine($"Toggle censor on Deck {deckId}");
        }
    }
}
