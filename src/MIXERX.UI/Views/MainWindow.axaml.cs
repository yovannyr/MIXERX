using Avalonia.Controls;

using Avalonia.Controls;
using Avalonia.Input;
using MIXERX.UI.Services;

namespace MIXERX.UI.Views;

public partial class MainWindow : Window
{
    private readonly KeyboardShortcutService _keyboardService;

    public MainWindow()
    {
        InitializeComponent();
        _keyboardService = new KeyboardShortcutService();
        
        // Enable keyboard input
        this.KeyDown += OnKeyDown;
        this.Focusable = true;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        _keyboardService.HandleKeyPress(e.Key, e.KeyModifiers);
        e.Handled = true;
    }
}
