using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using MIXERX.Core.Models;
using MIXERX.UI.Services;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace MIXERX.UI.ViewModels;

public class LibraryViewModel : ViewModelBase
{
    private readonly ILibraryService _libraryService;
    private string _searchText = string.Empty;
    private Track? _selectedTrack;

    public LibraryViewModel()
    {
        _libraryService = new LibraryService();
        Tracks = new ObservableCollection<Track>();
        
        SearchCommand = ReactiveCommand.CreateFromTask(Search);
        ImportDirectoryCommand = ReactiveCommand.CreateFromTask(ImportDirectory);
        LoadTrackCommand = ReactiveCommand.Create<Track>(LoadTrack);
        
        // Load tracks on startup
        _ = LoadAllTracks();
    }

    public ObservableCollection<Track> Tracks { get; }

    public string SearchText
    {
        get => _searchText;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            _ = Search(); // Auto-search on text change
        }
    }

    public Track? SelectedTrack
    {
        get => _selectedTrack;
        set => this.RaiseAndSetIfChanged(ref _selectedTrack, value);
    }

    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
    public ReactiveCommand<Unit, Unit> ImportDirectoryCommand { get; }
    public ReactiveCommand<Track, Unit> LoadTrackCommand { get; }

    private async Task Search()
    {
        var results = await _libraryService.SearchAsync(SearchText);
        
        Tracks.Clear();
        foreach (var track in results)
        {
            Tracks.Add(track);
        }
    }

    private async Task LoadAllTracks()
    {
        var tracks = await _libraryService.GetAllTracksAsync();
        
        Tracks.Clear();
        foreach (var track in tracks)
        {
            Tracks.Add(track);
        }
    }

    private async Task ImportDirectory()
    {
        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var topLevel = desktop?.MainWindow;
        
        if (topLevel?.StorageProvider != null)
        {
            var options = new FolderPickerOpenOptions
            {
                Title = "Select Music Directory",
                AllowMultiple = false
            };

            var result = await topLevel.StorageProvider.OpenFolderPickerAsync(options);
            var path = result.FirstOrDefault()?.Path.LocalPath;
            
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                await _libraryService.ImportDirectoryAsync(path);
                await LoadAllTracks();
            }
        }
    }

    private void LoadTrack(Track track)
    {
        // TODO: Send to deck via IPC
        SelectedTrack = track;
    }
}
