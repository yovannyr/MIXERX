namespace MIXERX.MAUI.Services;

public class FileDialogService : IFileDialogService
{
    private readonly Window _parentWindow;

    public FileDialogService(Window parentWindow)
    {
        _parentWindow = parentWindow;
    }

    public async Task<string?> OpenFileAsync(string title, params string[] extensions)
    {
        var storageProvider = _parentWindow.StorageProvider;
        
        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = CreateFileTypes(extensions)
        };

        var result = await storageProvider.OpenFilePickerAsync(options);
        return result.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<string?> OpenFolderAsync(string title)
    {
        var storageProvider = _parentWindow.StorageProvider;
        
        var options = new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        };

        var result = await storageProvider.OpenFolderPickerAsync(options);
        return result.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<IEnumerable<string>> OpenFilesAsync(string title, params string[] extensions)
    {
        var storageProvider = _parentWindow.StorageProvider;
        
        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = true,
            FileTypeFilter = CreateFileTypes(extensions)
        };

        var result = await storageProvider.OpenFilePickerAsync(options);
        return result.Select(f => f.Path.LocalPath);
    }

    private static FilePickerFileType[] CreateFileTypes(string[] extensions)
    {
        if (extensions.Length == 0)
            return [FilePickerFileTypes.All];

        return [new FilePickerFileType("Files") { Patterns = extensions.Select(ext => $"*{ext}").ToArray() }];
    }
}
