using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using MIXERX.Infrastructure.Services;
using MIXERX.Core.Interfaces;
using MIXERX.MAUI.Services;

namespace MIXERX.UI.ViewModels;

public class ControllerViewModel : ViewModelBase
{
    private readonly IMidiService _midiService;
    private readonly IControllerMapper _controllerMapper;
    private MidiDevice? _selectedDevice;
    private string _mappingScript = string.Empty;
    private bool _isConnected;

    public ControllerViewModel()
    {
        _midiService = new MidiService();
        _controllerMapper = new ControllerMapper();
        
        Devices = new ObservableCollection<MidiDevice>();
        
        RefreshDevicesCommand = ReactiveCommand.CreateFromTask(RefreshDevices);
        ConnectCommand = ReactiveCommand.CreateFromTask(Connect);
        DisconnectCommand = ReactiveCommand.CreateFromTask(Disconnect);
        LoadMappingCommand = ReactiveCommand.CreateFromTask(LoadMapping);
        
        // Subscribe to MIDI messages
        _midiService.MessageReceived += OnMidiMessageReceived;
        
        // Load devices on startup
        _ = RefreshDevices();
        
        // Load example mapping
        LoadExampleMapping();
    }

    public ObservableCollection<MidiDevice> Devices { get; }

    public MidiDevice? SelectedDevice
    {
        get => _selectedDevice;
        set => this.RaiseAndSetIfChanged(ref _selectedDevice, value);
    }

    public string MappingScript
    {
        get => _mappingScript;
        set => this.RaiseAndSetIfChanged(ref _mappingScript, value);
    }

    public bool IsConnected
    {
        get => _isConnected;
        set => this.RaiseAndSetIfChanged(ref _isConnected, value);
    }

    public ReactiveCommand<Unit, Unit> RefreshDevicesCommand { get; }
    public ReactiveCommand<Unit, Unit> ConnectCommand { get; }
    public ReactiveCommand<Unit, Unit> DisconnectCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadMappingCommand { get; }

    private async Task RefreshDevices()
    {
        var devices = await _midiService.GetDevicesAsync();
        
        Devices.Clear();
        foreach (var device in devices.Where(d => d.Type == MidiDeviceType.Input))
        {
            Devices.Add(device);
        }
    }

    private async Task Connect()
    {
        if (SelectedDevice != null)
        {
            await _midiService.ConnectAsync(SelectedDevice.Id);
            IsConnected = true;
            
            // Load current mapping
            if (!string.IsNullOrEmpty(MappingScript))
            {
                _controllerMapper.LoadMapping(MappingScript);
            }
        }
    }

    private async Task Disconnect()
    {
        await _midiService.DisconnectAsync();
        IsConnected = false;
    }

    private async Task LoadMapping()
    {
        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var topLevel = desktop?.MainWindow;
        
        if (topLevel?.StorageProvider != null)
        {
            var options = new FilePickerOpenOptions
            {
                Title = "Select Controller Mapping",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("JavaScript Files") { Patterns = ["*.js"] }]
            };

            var result = await topLevel.StorageProvider.OpenFilePickerAsync(options);
            var mappingPath = result.FirstOrDefault()?.Path.LocalPath;
            
            if (!string.IsNullOrEmpty(mappingPath) && File.Exists(mappingPath))
            {
                MappingScript = await File.ReadAllTextAsync(mappingPath);
                _controllerMapper.LoadMapping(MappingScript);
            }
        }
    }

    private void LoadExampleMapping()
    {
        MappingScript = @"
function onMidiMessage(msg) {
    const deck1 = getDeck(1);
    
    if (msg.isNoteOn(0x10)) {
        deck1.playPause();
        console.log('Deck 1 Play/Pause');
    }
    
    if (msg.isCC(0x30)) {
        const tempo = 0.5 + (msg.value * 1.5);
        deck1.setTempo(tempo);
        console.log('Deck 1 Tempo: ' + tempo);
    }
}";
    }

    private void OnMidiMessageReceived(MidiMessage message)
    {
        _controllerMapper.ProcessMidiMessage(message);
    }
}
