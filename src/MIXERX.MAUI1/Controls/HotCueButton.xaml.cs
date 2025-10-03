using System.Windows.Input;

namespace MIXERX.MAUI.Controls;

public partial class HotCueButton : ContentView
{
    public static readonly BindableProperty CueNumberProperty =
        BindableProperty.Create(nameof(CueNumber), typeof(int), typeof(HotCueButton), 1);

    public static readonly BindableProperty CueColorProperty =
        BindableProperty.Create(nameof(CueColor), typeof(Color), typeof(HotCueButton), Colors.Red);

    public static readonly BindableProperty CueLabelProperty =
        BindableProperty.Create(nameof(CueLabel), typeof(string), typeof(HotCueButton), string.Empty);

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(HotCueButton));

    public int CueNumber
    {
        get => (int)GetValue(CueNumberProperty);
        set => SetValue(CueNumberProperty, value);
    }

    public Color CueColor
    {
        get => (Color)GetValue(CueColorProperty);
        set => SetValue(CueColorProperty, value);
    }

    public string CueLabel
    {
        get => (string)GetValue(CueLabelProperty);
        set => SetValue(CueLabelProperty, value);
    }

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    public bool HasLabel => !string.IsNullOrEmpty(CueLabel);

    public HotCueButton()
    {
        InitializeComponent();
    }
}
