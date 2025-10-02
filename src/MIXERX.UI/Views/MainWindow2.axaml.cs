using Avalonia.Controls;
using MIXERX.UI.ViewModels;

namespace MIXERX.UI.Views;

public partial class MainWindow2 : Window
{
    public MainWindow2()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
