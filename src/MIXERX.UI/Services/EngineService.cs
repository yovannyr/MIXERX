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
    Task<DeckStatus?> GetStatusAsync(int deckId);
    bool IsConnected { get; }
}

public class EngineService : IEngineService, IDisposable
{
    private Process? _engineProcess;
    private NamedPipeClientStream? _pipeClient;
    private StreamReader? _reader;
    private StreamWriter? _writer;
    private readonly object _lock = new();

    public bool IsConnected => _pipeClient?.IsConnected == true;

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
