using System.IO.Pipes;
using System.Text;
using MIXERX.Core;

namespace MIXERX.Engine;

public class IpcServer : IDisposable
{
    private readonly AudioEngine _audioEngine;
    private NamedPipeServerStream? _pipeServer;
    private Thread? _serverThread;
    private volatile bool _isRunning;

    public Func<object, bool> OnCommand { get; set; }

    public IpcServer(AudioEngine audioEngine)
    {
        _audioEngine = audioEngine;
    }

    public void Start()
    {
        if (_isRunning) return;

        _isRunning = true;
        _serverThread = new Thread(ServerLoop)
        {
            Name = "MIXERX IPC Server",
            IsBackground = true
        };
        _serverThread.Start();
    }

    public void Stop()
    {
        _isRunning = false;
        _pipeServer?.Close();
        _serverThread?.Join(1000);
    }

    private void ServerLoop()
    {
        while (_isRunning)
        {
            try
            {
                _pipeServer = new NamedPipeServerStream("MIXERX_Engine", PipeDirection.InOut, 1);
                _pipeServer.WaitForConnection();

                using var reader = new StreamReader(_pipeServer, Encoding.UTF8);
                using var writer = new StreamWriter(_pipeServer, Encoding.UTF8) { AutoFlush = true };

                while (_pipeServer.IsConnected && _isRunning)
                {
                    var messageJson = reader.ReadLine();
                    if (string.IsNullOrEmpty(messageJson)) continue;

                    var message = IpcSerializer.Deserialize(messageJson);
                    if (message == null) continue;

                    var response = ProcessMessage(message, writer);
                    if (response != null)
                    {
                        writer.WriteLine(IpcSerializer.Serialize(response));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"IPC Server error: {ex.Message}");
            }
            finally
            {
                _pipeServer?.Dispose();
                _pipeServer = null;
            }

            if (_isRunning)
            {
                Thread.Sleep(100); // Brief pause before reconnecting
            }
        }
    }

    private IpcMessage? ProcessMessage(IpcMessage message, StreamWriter writer)
    {
        try
        {
            switch (message.Type)
            {
                case IpcMessageType.LoadTrack:
                    if (!string.IsNullOrEmpty(message.StringParam))
                    {
                        _audioEngine.LoadTrack(message.DeckId, message.StringParam);
                        // Send waveform data after track load
                        var waveformData = _audioEngine.GetWaveformData(message.DeckId);
                        if (waveformData != null && waveformData.Length > 0)
                        {
                            var waveformMsg = new WaveformDataMessage(message.DeckId, waveformData);
                            writer.WriteLine(IpcSerializer.Serialize(waveformMsg));
                        }
                    }
                    break;

                case IpcMessageType.Play:
                    _audioEngine.Play(message.DeckId);
                    break;

                case IpcMessageType.Pause:
                    _audioEngine.Pause(message.DeckId);
                    break;

                case IpcMessageType.SetTempo:
                    _audioEngine.SetTempo(message.DeckId, message.FloatParam);
                    break;

                case IpcMessageType.SetPosition:
                    _audioEngine.SetPosition(message.DeckId, TimeSpan.FromSeconds(message.FloatParam));
                    break;

                case IpcMessageType.SetEffectParameter:
                    if (message.Data != null && 
                        message.Data.TryGetValue("effectName", out var effectName) &&
                        message.Data.TryGetValue("paramName", out var paramName) &&
                        message.Data.TryGetValue("value", out var value))
                    {
                        _audioEngine.SetEffectParameter(message.DeckId, effectName.ToString()!, paramName.ToString()!, Convert.ToSingle(value));
                    }
                    break;

                case IpcMessageType.SetLoop:
                    if (message.Data != null && message.Data.TryGetValue("beats", out var beats))
                    {
                        _audioEngine.SetAutoLoop(message.DeckId, Convert.ToInt32(beats));
                    }
                    break;

                case IpcMessageType.ExitLoop:
                    _audioEngine.ExitLoop(message.DeckId);
                    break;

                case IpcMessageType.SetSync:
                    if (message.Data != null && message.Data.TryGetValue("enabled", out var enabled))
                    {
                        _audioEngine.SetSync(message.DeckId, Convert.ToBoolean(enabled));
                    }
                    break;

                case IpcMessageType.StartRecording:
                    if (!string.IsNullOrEmpty(message.StringParam))
                    {
                        _audioEngine.StartRecording(message.StringParam);
                    }
                    break;

                case IpcMessageType.StopRecording:
                    _audioEngine.StopRecording();
                    break;

                case IpcMessageType.GetStatus:
                    return GetDeckStatus(message.DeckId);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"IPC Message processing error: {ex.Message}");
        }

        return null;
    }

    private IpcMessage GetDeckStatus(int deckId)
    {
        // For now, return mock status
        // In production, this would query the actual deck state
        var status = new DeckStatus(
            IsPlaying: false,
            CurrentTrack: null,
            Position: 0,
            Duration: 0,
            Tempo: 1.0f,
            Volume: 1.0f
        );

        return new StatusResponseMessage(deckId, status);
    }

    public void Dispose()
    {
        Stop();
    }
}
