using MIXERX.MAUI.ViewModels;
using Xunit;

namespace MIXERX.MAUI.Tests.ViewModels;

public class DeckViewModelTests
{
    [Fact]
    public void PlayPause_TogglesIsPlaying()
    {
        var vm = new DeckViewModel();
        Assert.False(vm.IsPlaying);
        
        vm.PlayPauseCommand.Execute(null);
        Assert.True(vm.IsPlaying);
        
        vm.PlayPauseCommand.Execute(null);
        Assert.False(vm.IsPlaying);
    }

    [Fact]
    public void Sync_TogglesIsSynced()
    {
        var vm = new DeckViewModel();
        Assert.False(vm.IsSynced);
        
        vm.SyncCommand.Execute(null);
        Assert.True(vm.IsSynced);
    }

    [Fact]
    public void AutoLoop_SetsLoopLength()
    {
        var vm = new DeckViewModel();
        
        vm.AutoLoopCommand.Execute(8);
        Assert.Equal(8, vm.LoopLengthBeats);
        Assert.True(vm.IsLooping);
    }

    [Fact]
    public void HalveLoop_ReducesLoopLength()
    {
        var vm = new DeckViewModel { LoopLengthBeats = 8 };
        
        vm.HalveLoopCommand.Execute(null);
        Assert.Equal(4, vm.LoopLengthBeats);
    }

    [Fact]
    public void DoubleLoop_IncreasesLoopLength()
    {
        var vm = new DeckViewModel { LoopLengthBeats = 4 };
        
        vm.DoubleLoopCommand.Execute(null);
        Assert.Equal(8, vm.LoopLengthBeats);
    }
}
