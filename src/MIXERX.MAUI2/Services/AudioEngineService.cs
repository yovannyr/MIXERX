using System.Diagnostics;
using System.IO.Pipes;

namespace MIXERX.MAUI.Services;

public class AudioEngineService : IAudioEngineService
{
    private Process? _engineProcess;
    private NamedPipeClientStream? _pipeClient;
    private readonly bool _useInProcessEngine;

    public AudioEngineService()
    {
        // Use in-process engine for mobile, separate process for desktop
#if ANDROID || IOS
        _useInProcessEngine = true;
#else
        _useInProcessEngine = false;
#endif
    }

    public async Task StartEngineAsync()
    {
        if (_useInProcessEngine)
        {
            // TODO: Start in-process audio engine for mobile
            await Task.CompletedTask;
        }
        else
        {
            // Start separate engine process for desktop
            var enginePath = GetEnginePath();
            _engineProcess = Process.Start(new ProcessStartInfo
            {
                FileName = enginePath,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            // Connect to named pipe
            _pipeClient = new NamedPipeClientStream(".", "MIXERX_Engine", PipeDirection.InOut);
            await _pipeClient.ConnectAsync(5000);
        }
    }

    public async Task StopEngineAsync()
    {
        if (_pipeClient != null)
        {
            await _pipeClient.DisposeAsync();
            _pipeClient = null;
        }

        if (_engineProcess != null && !_engineProcess.HasExited)
        {
            _engineProcess.Kill();
            _engineProcess.Dispose();
            _engineProcess = null;
        }
    }

    public Task<bool> IsEngineRunningAsync()
    {
        if (_useInProcessEngine)
            return Task.FromResult(true);

        return Task.FromResult(_engineProcess != null && !_engineProcess.HasExited);
    }

    public async Task SendCommandAsync(string command)
    {
        if (_pipeClient?.IsConnected == true)
        {
            var writer = new StreamWriter(_pipeClient) { AutoFlush = true };
            await writer.WriteLineAsync(command);
        }
    }

    private string GetEnginePath()
    {
#if WINDOWS
        return Path.Combine(AppContext.BaseDirectory, "MIXERX.Engine.exe");
#elif MACCATALYST
        return Path.Combine(AppContext.BaseDirectory, "MIXERX.Engine");
#else
        throw new PlatformNotSupportedException();
#endif
    }
}
