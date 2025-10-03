using MIXERX.App.Models;
using MIXERX.App.PageModels;

namespace MIXERX.App.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}