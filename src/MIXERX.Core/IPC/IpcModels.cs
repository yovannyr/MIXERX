using System.Windows.Input;

namespace MIXERX.Core.IPC;

public class IpcCommand: ICommand
{
    public string Type { get; set; } = "";
    public int DeckId { get; set; }
    public string? StringParam { get; set; }
    public float FloatParam { get; set; }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        throw new NotImplementedException();
    }
}

public class IpcResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public object? Data { get; set; }
}
