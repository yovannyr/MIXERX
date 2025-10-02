using Xunit;
using MIXERX.UI.Services;

namespace MIXERX.UI.Tests
{
    public class KeyboardShortcutTests
    {
        // [Fact]
        // public void KeyboardShortcutService_ShouldHandlePlayPause()
        // {
        //     var service = new KeyboardShortcutService();
        //     
        //     Assert.NotNull(service);
        //     Assert.True(service.IsShortcutRegistered("Space"));
        // }

        [Fact]
        public void KeyboardShortcutService_ShouldHandleHotCues()
        {
            var service = new KeyboardShortcutService();
            
            Assert.True(service.IsShortcutRegistered("Q"));
            Assert.True(service.IsShortcutRegistered("W"));
            Assert.True(service.IsShortcutRegistered("E"));
            Assert.True(service.IsShortcutRegistered("R"));
        }
    }
}
