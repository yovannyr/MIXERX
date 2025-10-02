namespace MIXERX.MAUI.Views;

public partial class MainPage2 : ContentPage
{
    public MainPage2()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetService<ViewModels.MainViewModel>();
    }
}
