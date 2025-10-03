using MIXERX.MAUI.ViewModels;

namespace MIXERX.MAUI.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
