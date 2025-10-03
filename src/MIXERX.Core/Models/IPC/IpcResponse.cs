namespace MIXERX.Core.Models.IPC;

public class IpcResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public object? Data { get; set; }
}