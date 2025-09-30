using Avalonia.Data.Converters;
using System.Globalization;

namespace MIXERX.UI.Converters;

public static class BoolConverters
{
    public static readonly IValueConverter PlayPause = new FuncValueConverter<bool, string>(
        isPlaying => isPlaying ? "⏸️ Pause" : "▶️ Play"
    );

    public static readonly IValueConverter EngineStatus = new FuncValueConverter<bool, string>(
        isRunning => isRunning ? "Engine Running" : "Engine Stopped");
}
