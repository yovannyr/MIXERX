using MIXERX.Core;
using MIXERX.Engine;

Console.WriteLine("MIXERX Audio Engine Starting...");

var engine = new AudioEngine();

var config = new AudioConfig
{
    SampleRate = 48000,
    BufferSize = 128,
    PreferredApi = OperatingSystem.IsWindows() ? AudioApi.Wasapi : AudioApi.CoreAudio
};

Console.WriteLine($"Initializing audio with {config.PreferredApi} API...");

try
{
    if (await engine.StartAsync(config))
    {
        Console.WriteLine("Audio engine started successfully!");
        Console.WriteLine("IPC server started for UI communication.");
        Console.WriteLine("MIXERX Engine is running. Press 'q' to quit.");
        
        // Keep engine running
        while (Console.ReadKey().KeyChar != 'q')
        {
            await Task.Delay(1000);
        }
        
        Console.WriteLine("\nShutting down...");
        await engine.StopAsync();
        Console.WriteLine("Engine stopped.");
    }
    else
    {
        Console.WriteLine("Failed to start audio engine. This is expected in demo mode without audio hardware.");
        Console.WriteLine("Engine would normally initialize audio drivers here.");
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
        return 1;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error starting engine: {ex.Message}");
    Console.WriteLine("This is expected in demo mode without proper audio drivers.");
    Console.WriteLine("Press any key to exit.");
    Console.ReadKey();
    return 1;
}

return 0;
