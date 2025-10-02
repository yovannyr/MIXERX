using MIXERX.MAUI.ViewModels;
using Xunit;

namespace MIXERX.MAUI.Tests.ViewModels;

public class SamplerViewModelTests
{
    [Fact]
    public void SelectBank_ChangesCurrentBank()
    {
        var vm = new SamplerViewModel();
        Assert.Equal(0, vm.CurrentBank);
        
        vm.SelectBankCommand.Execute(2);
        Assert.Equal(2, vm.CurrentBank);
    }

    [Fact]
    public void GetSlot_ReturnsCorrectSlot()
    {
        var vm = new SamplerViewModel();
        var slot = vm.GetSlot(1, 3);
        
        Assert.NotNull(slot);
        Assert.Equal("Slot 4", slot.Name);
    }

    [Fact]
    public void TriggerSample_TogglesPlayingState()
    {
        var vm = new SamplerViewModel();
        var slot = vm.GetCurrentSlot(0);
        slot.FilePath = "test.wav";
        
        vm.TriggerSampleCommand.Execute(0);
        Assert.True(slot.IsPlaying);
        
        vm.TriggerSampleCommand.Execute(0);
        Assert.False(slot.IsPlaying);
    }
}
