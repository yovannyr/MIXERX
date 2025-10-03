using Xunit;
using MIXERX.Engine;
using MIXERX.Core;
using MIXERX.Core.Interfaces;

namespace MIXERX.Engine.Tests;

public class Iteration1Tests
{
    [Fact]
    public void MockAudioDriver_CanBeCreated()
    {
        var driver = new MockAudioDriver();
        Assert.NotNull(driver);
    }

    [Fact]
    public void MockAudioDriver_Initialize_ReturnsTrue()
    {
        var driver = new MockAudioDriver();
        var config = new AudioConfig();
        
        var result = driver.Initialize(config);
        
        Assert.True(result);
    }

    [Fact]
    public void MockAudioDriver_GetDevices_ReturnsDevices()
    {
        var driver = new MockAudioDriver();
        
        var devices = driver.GetDevices();
        
        Assert.NotEmpty(devices);
        Assert.Equal("Mock Device", devices[0].Name);
    }
}
