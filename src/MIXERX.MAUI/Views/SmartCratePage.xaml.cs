namespace MIXERX.MAUI.Views;

public partial class SmartCratePage : ContentPage
{
    public SmartCratePage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.SmartCrateViewModel();
    }
}
