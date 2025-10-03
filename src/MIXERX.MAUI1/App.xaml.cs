namespace MIXERX.MAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Register Syncfusion license
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("YOUR_LICENSE_KEY_HERE");

        MainPage = new AppShell();
    }
}
