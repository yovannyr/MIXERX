using MIXERX.MAUI.ViewModels;

namespace MIXERX.MAUI.Views;

public partial class LibraryPage : ContentPage
{
    public LibraryPage(LibraryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
