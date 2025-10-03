namespace MIXERX.MAUI.Services;

public interface IKeyboardShortcutService
{
    void RegisterShortcuts();
    bool IsShortcutRegistered(string key);
    void HandleKeyPress(Key key, KeyModifiers modifiers);

}

public interface Key
{
}


public interface KeyModifiers
{
}