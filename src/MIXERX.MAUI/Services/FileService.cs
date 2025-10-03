using MIXERX.Core.Models.IPC;
using MIXERX.MAUI.Services.IPC;

namespace MIXERX.MAUI.Services;

public class FileService
{
    private readonly IpcClient _ipcClient;

    public FileService()
    {
        _ipcClient = new IpcClient();
        _ipcClient.Connect("mixerx-engine");
    }

    public async Task<string?> OpenAudioFile()
    {
        var desktop = App.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
        var topLevel = TopLevel.GetTopLevel(desktop?.MainWindow);
        if (topLevel?.StorageProvider == null) return null;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Audio File",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Audio Files")
                {
                    Patterns = new[] { "*.wav", "*.mp3", "*.flac" }
                }
            }
        });

        return files.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task LoadTrackToDeck(string path, int deckId)
    {
        var command = new IpcCommand 
        { 
            Type = "LoadTrack", 
            DeckId = deckId, 
            StringParam = path 
        };
        
        _ipcClient.SendCommand(command);
    }

    public async Task PlayDeck(int deckId)
    {
        var command = new IpcCommand { Type = "Play", DeckId = deckId };
        _ipcClient.SendCommand(command);
    }

    public async Task PauseDeck(int deckId)
    {
        var command = new IpcCommand { Type = "Pause", DeckId = deckId };
        _ipcClient.SendCommand(command);
    }
}
