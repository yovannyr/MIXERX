using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MIXERX.MAUI.Models;
using System.Collections.ObjectModel;

namespace MIXERX.MAUI.ViewModels;

public partial class SmartCrateViewModel : ObservableObject
{
    [ObservableProperty]
    private string crateName = "New Smart Crate";

    [ObservableProperty]
    private CrateLogic logic = CrateLogic.And;

    public ObservableCollection<CrateRule> Rules { get; } = new();
    public ObservableCollection<TrackViewModel> MatchingTracks { get; } = new();

    [RelayCommand]
    private void AddRule()
    {
        Rules.Add(new CrateRule());
    }

    [RelayCommand]
    private void RemoveRule(CrateRule rule)
    {
        Rules.Remove(rule);
    }

    [RelayCommand]
    private async Task ApplyRulesAsync()
    {
        MatchingTracks.Clear();
        // TODO: Filter tracks based on rules
        await Task.CompletedTask;
    }
}
