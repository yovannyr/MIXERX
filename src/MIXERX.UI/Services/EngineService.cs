using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using MIXERX.Core;

namespace MIXERX.UI.Services;

public interface IEngineService
{
    Task<bool> StartEngineAsync();
    Task StopEngineAsync();
    Task LoadTrackAsync(int deckId, string filePath);
    Task PlayAsync(int deckId);
    Task PauseAsync(int deckId);
    Task SetTempoAsync(int deckId, float tempo);
    Task SetPositionAsync(int deckId, float position);
    Task SetCuePointAsync(int deckId, float position);
    Task SetEffectParameterAsync(int deckId, string effectName, string paramName, float value);
    Task<DeckStatus?> GetStatusAsync(int deckId);
    Task StartRecordingAsync(string filePath);
    Task StopRecordingAsync();
    Task SetAutoLoopAsync(int deckId, int beats);
    Task ExitLoopAsync(int deckId);
    Task SetSyncAsync(int deckId, bool enabled);
    bool IsConnected { get; }
    event Action<int, float[]>? WaveformDataReceived;
    event Action<int, bool, int, float>? LoopStatusReceived;
    event Action<int, float>? BpmDetected;
}

public class EngineService : IEngineService, IDisposable
{
    private Process? _engineProcess;
    private NamedPipeClientStream? _pipeClient;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    private readonly object _lock = new();

    public bool IsConnected => _pipeClient?.IsConnected == true;
    
    public event Action<int, float[]>? WaveformDataReceived;

    public async Task<bool> StartEngineAsync()
    {
        try
        {
            // Start engine process
            var enginePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MIXERX.Engine.exe");
            if (!File.Exists(enginePath))
            {
                // Try relative path for development
                enginePath = Path.Combine("..", "..", "..", "..", "MIXERX.Engine", "bin", "Debug", "net9.0", "MIXERX.Engine.exe");
            }

            if (!File.Exists(enginePath))
            {
                System.Diagnostics.Debug.WriteLine("Engine executable not found");
                return false;
            }

            _engineProcess = Process.Start(new ProcessStartInfo
            {
                FileName = enginePath,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            // Wait for engine to start
            await Task.Delay(2000);

            // Connect to engine via named pipe
            _pipeClient = new NamedPipeClientStream(".", "MIXERX_Engine", PipeDirection.InOut);
            await _pipeClient.ConnectAsync(5000);

            _reader = new StreamReader(_pipeClient, Encoding.UTF8);
            _writer = new StreamWriter(_pipeClient, Encoding.UTF8) { AutoFlush = true };

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to start engine: {ex.Message}");
            return false;
        }
    }

    public async Task StopEngineAsync()
    {
        lock (_lock)
        {
            _reader?.Dispose();
            _writer?.Dispose();
            _pipeClient?.Dispose();
            
            _reader = null;
            _writer = null;
            _pipeClient = null;
        }

        if (_engineProcess != null && !_engineProcess.HasExited)
        {
            _engineProcess.Kill();
            await _engineProcess.WaitForExitAsync();
        }

        _engineProcess?.Dispose();
        _engineProcess = null;
    }

    public async Task LoadTrackAsync(int deckId, string filePath)
    {
        var message = new LoadTrackMessage(deckId, filePath);
        await SendMessageAsync(message);
        
        // Wait for waveform data response
        await Task.Run(() =>
        {
            try
            {
                lock (_lock)
                {
                    if (_reader != null && IsConnected)
                    {
                        var responseJson = _reader.ReadLine();
                        if (!string.IsNullOrEmpty(responseJson))
                        {
                            var response = IpcSerializer.Deserialize(responseJson);
                            if (response?.Type == IpcMessageType.WaveformData && response.Data != null)
                            {
                                if (response.Data.TryGetValue("waveform", out var waveformObj))
                                {
                                    var waveformData = waveformObj as float[] ?? Array.Empty<float>();
                                    WaveformDataReceived?.Invoke(deckId, waveformData);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors
            }
        });
    }

    public async Task PlayAsync(int deckId)
    {
        var message = new PlayMessage(deckId);
        await SendMessageAsync(message);
    }

    public async Task PauseAsync(int deckId)
    {
        var message = new PauseMessage(deckId);
        await SendMessageAsync(message);
    }

    public async Task SetTempoAsync(int deckId, float tempo)
    {
        var message = new SetTempoMessage(deckId, tempo);
        await SendMessageAsync(message);
    }

    public async Task SetPositionAsync(int deckId, float position)
    {
        var message = new SetPositionMessage(deckId, position);
        await SendMessageAsync(message);
    }

    public async Task SetCuePointAsync(int deckId, float position)
    {
        var message = new SetCuePointMessage(deckId, position);
        await SendMessageAsync(message);
    }

    public async Task SetEffectParameterAsync(int deckId, string effectName, string paramName, float value)
    {
        var message = new SetEffectParameterMessage(deckId, effectName, paramName, value);
        await SendMessageAsync(message);
    }

    public async Task<DeckStatus?> GetStatusAsync(int deckId)
    {
        var message = new GetStatusMessage(deckId);
        var response = await SendMessageWithResponseAsync(message);
        
        if (response?.Type == IpcMessageType.StatusResponse && response.Data != null)
        {
            // In production, properly deserialize the status
            return new DeckStatus(false, null, 0, 0, 1.0f, 1.0f);
        }

        return null;
    }

    public async Task StartRecordingAsync(string filePath)
    {
        var message = new StartRecordingMessage(filePath);
        await SendMessageAsync(message);
    }

    public async Task StopRecordingAsync()
    {
        var message = new StopRecordingMessage();
        await SendMessageAsync(message);
    }

    public async Task SetAutoLoopAsync(int deckId, int beats)
    {
        var message = new SetLoopMessage(deckId, beats);
        await SendMessageAsync(message);
    }

    public async Task ExitLoopAsync(int deckId)
    {
        var message = new ExitLoopMessage(deckId);
        await SendMessageAsync(message);
    }

    public async Task SetSyncAsync(int deckId, bool enabled)
    {
        var message = new SetSyncMessage(deckId, enabled);
        await SendMessageAsync(message);
    }
    
    public event Action<int, bool, int, float>? LoopStatusReceived;
    public event Action<int, float>? BpmDetected;

    private Task SendMessageAsync(IpcMessage message)
    {
        lock (_lock)
        {
            if (_writer == null || !IsConnected) return Task.CompletedTask;

            try
            {
                var json = IpcSerializer.Serialize(message);
                _writer.WriteLine(json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"IPC send error: {ex.Message}");
            }
        }
        return Task.CompletedTask;
    }

    private Task<IpcMessage?> SendMessageWithResponseAsync(IpcMessage message)
    {
        lock (_lock)
        {
            if (_writer == null || _reader == null || !IsConnected) return Task.FromResult<IpcMessage?>(null);

            try
            {
                var json = IpcSerializer.Serialize(message);
                _writer.WriteLine(json);

                var responseJson = _reader.ReadLine();
                return Task.FromResult(string.IsNullOrEmpty(responseJson) ? null : IpcSerializer.Deserialize(responseJson));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"IPC request error: {ex.Message}");
                return Task.FromResult<IpcMessage?>(null);
            }
        }
    }

    public void Dispose()
    {
        StopEngineAsync().Wait(1000);
    }
}
