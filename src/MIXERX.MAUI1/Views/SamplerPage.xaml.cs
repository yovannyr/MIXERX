using Microsoft.Maui.Controls;

namespace MIXERX.MAUI.Views;

public partial class SamplerPage : ContentPage
{
    public SamplerPage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.SamplerViewModel();
    }

    private void InitializeComponent()
    {
    }
}
