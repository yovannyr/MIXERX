using Avalonia.Controls;
using MIXERX.UI.ViewModels;

namespace MIXERX.UI.Views;

public partial class DeckView : UserControl
{
    public DeckView()
    {
        InitializeComponent();

        // Wire up HotCueButton events
        HotCueButton1.HotCueTriggered += OnHotCueTriggered;
        HotCueButton1.HotCueSet += OnHotCueSet;
        HotCueButton1.HotCueDeleted += OnHotCueDeleted;

        HotCueButton2.HotCueTriggered += OnHotCueTriggered;
        HotCueButton2.HotCueSet += OnHotCueSet;
        HotCueButton2.HotCueDeleted += OnHotCueDeleted;

        HotCueButton3.HotCueTriggered += OnHotCueTriggered;
        HotCueButton3.HotCueSet += OnHotCueSet;
        HotCueButton3.HotCueDeleted += OnHotCueDeleted;

        HotCueButton4.HotCueTriggered += OnHotCueTriggered;
        HotCueButton4.HotCueSet += OnHotCueSet;
        HotCueButton4.HotCueDeleted += OnHotCueDeleted;
    }

    private void OnHotCueTriggered(object? sender, int cueNumber)
    {
        if (DataContext is DeckViewModel viewModel)
        {
            _ = viewModel.TriggerHotCue(cueNumber);
        }
    }

    private void OnHotCueSet(object? sender, int cueNumber)
    {
        if (DataContext is DeckViewModel viewModel)
        {
            _ = viewModel.SetHotCue(cueNumber);
        }
    }

    private void OnHotCueDeleted(object? sender, int cueNumber)
    {
        if (DataContext is DeckViewModel viewModel)
        {
            _ = viewModel.DeleteHotCue(cueNumber);
        }
    }
}
