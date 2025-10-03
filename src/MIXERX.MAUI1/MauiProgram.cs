using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;

namespace MIXERX.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .ConfigureSyncfusionToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services
        builder.Services.AddSingleton<Services.IAudioEngineService, Services.AudioEngineService>();
        
        // Register platform-specific audio service
#if WINDOWS
        builder.Services.AddSingleton<Services.IPlatformAudioService, Platforms.Windows.WindowsAudioService>();
#elif MACCATALYST
        builder.Services.AddSingleton<Services.IPlatformAudioService, Platforms.MacCatalyst.MacAudioService>();
#elif ANDROID
        builder.Services.AddSingleton<Services.IPlatformAudioService, Platforms.Android.AndroidAudioService>();
#elif IOS
        builder.Services.AddSingleton<Services.IPlatformAudioService, Platforms.iOS.iOSAudioService>();
#endif
        
        // Register ViewModels
        builder.Services.AddTransient<ViewModels.MainViewModel>();
        builder.Services.AddTransient<ViewModels.LibraryViewModel>();
        builder.Services.AddTransient<ViewModels.SettingsViewModel>();
        
        // Register Views
        builder.Services.AddTransient<Views.MainPage>();
        builder.Services.AddTransient<Views.MainPage2>();
        builder.Services.AddTransient<Views.LibraryPage>();
        builder.Services.AddTransient<Views.SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

