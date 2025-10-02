namespace MIXERX.MAUI.Views;

public partial class StreamingPage : ContentPage
{
    public StreamingPage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.StreamingViewModel();
    }
}
