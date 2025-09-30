using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MIXERX.UI.ViewModels;
using MIXERX.UI.Views;
using MIXERX.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MIXERX.UI;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        ConfigureServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            
            // Register FileDialogService with MainWindow
            var services = new ServiceCollection();
            services.AddSingleton<IFileDialogService>(new FileDialogService(mainWindow));
            _serviceProvider = services.BuildServiceProvider();
            
            mainWindow.DataContext = new MainWindowViewModel();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices()
    {
        // Service configuration will be expanded later
    }

    public T? GetService<T>() where T : class
    {
        return _serviceProvider?.GetService<T>();
    }
}
