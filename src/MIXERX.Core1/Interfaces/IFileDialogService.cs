namespace MIXERX.MAUI.Services;

public interface IFileDialogService
{
    Task<string?> OpenFileAsync(string title, params string[] extensions);
    Task<string?> OpenFolderAsync(string title);
    Task<IEnumerable<string>> OpenFilesAsync(string title, params string[] extensions);
}