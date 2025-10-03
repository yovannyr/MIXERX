using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIXERX.MAUI.Services;

namespace MIXERX.MAUI.ViewModels;

public partial class DVSViewModel : ObservableObject
{
    private readonly IDVSService _dvsService = new DVSService();

    [ObservableProperty]
    private bool isEnabled;

    [ObservableProperty]
    private string timecodeType = "Serato";

    [ObservableProperty]
    private float pitch = 1.0f;

    [RelayCommand]
    private void ToggleDVS()
    {
        IsEnabled = !IsEnabled;
        _dvsService.IsEnabled = IsEnabled;
    }

    [RelayCommand]
    private void Calibrate()
    {
        _dvsService.Calibrate();
    }
}
