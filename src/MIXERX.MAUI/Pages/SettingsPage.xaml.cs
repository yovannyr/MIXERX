using Microsoft.Maui.Controls;
using MIXERX.MAUI.ViewModels;

namespace MIXERX.MAUI.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void InitializeComponent()
    {
    }
}
