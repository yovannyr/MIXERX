using Microsoft.Maui.Controls;

namespace MIXERX.MAUI.Views;

public partial class MainPage2 : ContentPage
{
    public MainPage2(ViewModels.MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void InitializeComponent()
    {
    }
}
