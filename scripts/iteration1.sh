#!/bin/bash
# MIXERX Iteration 1: Audio Foundation
# Zero-Break TDD Implementation

set -e

echo "🎯 Starting Iteration 1: Audio Foundation"

# 1. Create test structure
echo "📝 Creating test files..."
mkdir -p tests/MIXERX.Engine.Tests

# 2. Red Phase: Write failing tests
cat > tests/MIXERX.Engine.Tests/AudioDriverTests.cs << 'EOF'
using Xunit;
using MIXERX.Engine;
using MIXERX.Core;

namespace MIXERX.Engine.Tests;

public class AudioDriverTests
{
    [Fact]
    public void MockAudioDriver_Initialize_ReturnsTrue()
    {
        var driver = new MockAudioDriver(new AudioConfig());
        Assert.True(driver.Initialize());
    }

    [Fact]
    public void MockAudioDriver_Start_WithValidConfig_Succeeds()
    {
        var driver = new MockAudioDriver(new AudioConfig());
        driver.Initialize();
        Assert.True(driver.Start());
    }

    [Fact]
    public void MockAudioDriver_ProcessBuffer_CallsCallback()
    {
        var driver = new MockAudioDriver(new AudioConfig());
        var callbackCalled = false;
        
        driver.SetCallback((buffer, frames) => callbackCalled = true);
        driver.ProcessBuffer();
        
        Assert.True(callbackCalled);
    }
}
EOF

# 3. Update project file
cat > tests/MIXERX.Engine.Tests/MIXERX.Engine.Tests.csproj << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="../../src/MIXERX.Engine/MIXERX.Engine.csproj" />
    <ProjectReference Include="../../src/MIXERX.Core/MIXERX.Core.csproj" />
  </ItemGroup>
</Project>
EOF

# 4. Run tests (should fail)
echo "🔴 Running tests (should fail)..."
dotnet test tests/MIXERX.Engine.Tests/ || echo "✅ Tests failed as expected"

# 5. Green Phase: Minimal implementation
echo "🟢 Implementing minimal code..."

# Add MockAudioDriver to AudioDrivers.cs
cat >> src/MIXERX.Engine/AudioDrivers.cs << 'EOF'

public class MockAudioDriver : IAudioDriver
{
    private readonly AudioConfig _config;
    private Action<float[], int>? _callback;
    private bool _initialized;
    private bool _running;

    public MockAudioDriver(AudioConfig config)
    {
        _config = config;
    }

    public bool Initialize()
    {
        _initialized = true;
        return true;
    }

    public bool Start()
    {
        if (!_initialized) return false;
        _running = true;
        return true;
    }

    public void Stop()
    {
        _running = false;
    }

    public void SetCallback(Action<float[], int> callback)
    {
        _callback = callback;
    }

    public void ProcessBuffer()
    {
        if (_running && _callback != null)
        {
            var buffer = new float[_config.BufferSize];
            _callback(buffer, _config.BufferSize);
        }
    }

    public void Dispose()
    {
        Stop();
    }
}
EOF

# 6. Run tests (should pass)
echo "🟢 Running tests (should pass)..."
dotnet test tests/MIXERX.Engine.Tests/

# 7. Integration test
echo "🔧 Testing integration..."
dotnet build
dotnet test

echo "✅ Iteration 1 completed successfully!"
echo "📊 Status: Audio Driver foundation implemented"
echo "🎯 Next: Run ./scripts/iteration2.sh"
