using System.Text.Json;

namespace MIXERX.Core.Models.IPC;

public static class IpcSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static string Serialize(IpcMessage message)
    {
        return JsonSerializer.Serialize(message, Options);
    }

    public static IpcMessage? Deserialize(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<IpcMessage>(json, Options);
        }
        catch
        {
            return null;
        }
    }
}