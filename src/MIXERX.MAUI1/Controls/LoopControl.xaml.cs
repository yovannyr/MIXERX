using System.Windows.Input;

namespace MIXERX.MAUI.Controls;

public partial class LoopControl : ContentView
{
    public static readonly BindableProperty SetLoopCommandProperty =
        BindableProperty.Create(nameof(SetLoopCommand), typeof(ICommand), typeof(LoopControl));

    public static readonly BindableProperty HalveLoopCommandProperty =
        BindableProperty.Create(nameof(HalveLoopCommand), typeof(ICommand), typeof(LoopControl));

    public static readonly BindableProperty DoubleLoopCommandProperty =
        BindableProperty.Create(nameof(DoubleLoopCommand), typeof(ICommand), typeof(LoopControl));

    public static readonly BindableProperty ExitLoopCommandProperty =
        BindableProperty.Create(nameof(ExitLoopCommand), typeof(ICommand), typeof(LoopControl));

    public ICommand SetLoopCommand
    {
        get => (ICommand)GetValue(SetLoopCommandProperty);
        set => SetValue(SetLoopCommandProperty, value);
    }

    public ICommand HalveLoopCommand
    {
        get => (ICommand)GetValue(HalveLoopCommandProperty);
        set => SetValue(HalveLoopCommandProperty, value);
    }

    public ICommand DoubleLoopCommand
    {
        get => (ICommand)GetValue(DoubleLoopCommandProperty);
        set => SetValue(DoubleLoopCommandProperty, value);
    }

    public ICommand ExitLoopCommand
    {
        get => (ICommand)GetValue(ExitLoopCommandProperty);
        set => SetValue(ExitLoopCommandProperty, value);
    }

    public LoopControl()
    {
        InitializeComponent();
    }
}
