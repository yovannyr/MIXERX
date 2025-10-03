$proj = ".\src\MIXERX.MAUI\MIXERX.MAUI.csproj"

# Paketliste (IDs auf NuGet); Version optional
$packages = @(
    @{ Id = "NWaves";                    Version = "" }           # DSP/Analyse
    @{ Id = "RtMidi.Core";               Version = "" }           # MIDI
    @{ Id = "HidSharp";                  Version = "" }           # HID-Controller
    @{ Id = "Concentus";                 Version = "" }           # Opus (falls benötigt)
    @{ Id = "SkiaSharp";                 Version = "" }           # Rendering
    @{ Id = "SkiaSharp.Views.Maui.Controls"; Version = "" }       # SkiaSharp in MAUI
    # Falls du ein Miniaudio-Binding aus NuGet nutzt, ergänze hier die korrekte ID:
    # @{ Id = "Miniaudio.*";             Version = "" }
)

foreach ($p in $packages) {
    if ([string]::IsNullOrWhiteSpace($p.Version)) {
        dotnet add $proj package $p.Id
    } else {
        dotnet add $proj package $p.Id --version $p.Version
    }
}
