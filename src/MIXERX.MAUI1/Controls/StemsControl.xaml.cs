namespace MIXERX.MAUI.Controls;

public partial class StemsControl : ContentView
{
    public StemsControl()
    {
        InitializeComponent();
        BindingContext = new ViewModels.StemsViewModel();
    }
}
