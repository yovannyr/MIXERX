using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIXERX.MAUI.Models;

namespace MIXERX.MAUI.ViewModels;

public partial class SamplerViewModel : ObservableObject
{
    private readonly SampleSlot[,] _samples = new SampleSlot[4, 8]; // 4 banks, 8 slots

    [ObservableProperty]
    private int currentBank = 0;

    public SamplerViewModel()
    {
        // Initialize all slots
        for (int bank = 0; bank < 4; bank++)
        {
            for (int slot = 0; slot < 8; slot++)
            {
                _samples[bank, slot] = new SampleSlot { Name = $"Slot {slot + 1}" };
            }
        }
    }

    public SampleSlot GetSlot(int bank, int slot) => _samples[bank, slot];
    public SampleSlot GetCurrentSlot(int slot) => _samples[CurrentBank, slot];

    [RelayCommand]
    private void SelectBank(int bank)
    {
        CurrentBank = bank;
        OnPropertyChanged(nameof(CurrentBank));
    }

    [RelayCommand]
    private async Task LoadSampleAsync(int slot)
    {
        // TODO: File picker
        await Task.CompletedTask;
    }

    [RelayCommand]
    private void TriggerSample(int slot)
    {
        var sample = GetCurrentSlot(slot);
        if (sample.IsLoaded)
        {
            sample.IsPlaying = !sample.IsPlaying;
        }
    }

    [RelayCommand]
    private void ClearSample(int slot)
    {
        var sample = GetCurrentSlot(slot);
        sample.FilePath = null;
        sample.IsPlaying = false;
    }
}
