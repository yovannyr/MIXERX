using Microsoft.Maui.Controls;

namespace MIXERX.MAUI.Views;

public partial class DVSPage : ContentPage
{
    public DVSPage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.DVSViewModel();
    }

    private void InitializeComponent()
    {
    }
}
