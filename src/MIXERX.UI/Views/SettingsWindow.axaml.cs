using Avalonia.Controls;
using Avalonia.Interactivity;
using MIXERX.Core.Settings;

namespace MIXERX.UI.Views;

public partial class SettingsWindow : Window
{
    private AppSettings _settings;

    public SettingsWindow()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        _settings = SettingsService.Load();
        // TODO: Bind settings to UI controls
    }

    private void ApplyButton_Click(object? sender, RoutedEventArgs e)
    {
        // TODO: Read values from UI controls and update _settings
        SettingsService.Save(_settings);
        Close();
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        SettingsService.Reset();
        LoadSettings();
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
