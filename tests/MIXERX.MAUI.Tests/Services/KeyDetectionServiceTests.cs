using MIXERX.MAUI.Services;
using Xunit;

namespace MIXERX.MAUI.Tests.Services;

public class KeyDetectionServiceTests
{
    [Theory]
    [InlineData("Am", "8A")]
    [InlineData("C", "8B")]
    [InlineData("G", "9B")]
    [InlineData("Em", "9A")]
    public void GetHarmonicKey_ReturnsCorrectCamelot(string musicalKey, string expectedCamelot)
    {
        var service = new KeyDetectionService();
        var result = service.GetHarmonicKey(musicalKey);
        
        Assert.Equal(expectedCamelot, result);
    }

    [Theory]
    [InlineData("Am", "Am", true)]  // Same key
    [InlineData("Am", "Em", true)]  // +1
    [InlineData("Am", "C", true)]   // Energy boost
    [InlineData("Am", "D", false)]  // Not compatible
    public void AreKeysCompatible_ReturnsCorrectResult(string key1, string key2, bool expected)
    {
        var service = new KeyDetectionService();
        var result = service.AreKeysCompatible(key1, key2);
        
        Assert.Equal(expected, result);
    }
}
