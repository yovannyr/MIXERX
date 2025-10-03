using Microsoft.Maui.Controls;

namespace MIXERX.MAUI.Views;

public partial class SmartCratePage : ContentPage
{
    public SmartCratePage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.SmartCrateViewModel();
    }

    private void InitializeComponent()
    {
    }
}
