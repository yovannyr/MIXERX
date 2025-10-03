using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace MIXERX.UI.Controls;

public class EQControl : UserControl
{
    public static readonly StyledProperty<double> HighProperty =
        AvaloniaProperty.Register<EQControl, double>(nameof(High), 0.5);

    public static readonly StyledProperty<double> MidProperty =
        AvaloniaProperty.Register<EQControl, double>(nameof(Mid), 0.5);

    public static readonly StyledProperty<double> LowProperty =
        AvaloniaProperty.Register<EQControl, double>(nameof(Low), 0.5);

    public double High
    {
        get => GetValue(HighProperty);
        set => SetValue(HighProperty, Math.Clamp(value, 0.0, 1.0));
    }

    public double Mid
    {
        get => GetValue(MidProperty);
        set => SetValue(MidProperty, Math.Clamp(value, 0.0, 1.0));
    }

    public double Low
    {
        get => GetValue(LowProperty);
        set => SetValue(LowProperty, Math.Clamp(value, 0.0, 1.0));
    }

    public EQControl()
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*,*"),
            RowDefinitions = new RowDefinitions("Auto,*")
        };

        // Labels
        var highLabel = new TextBlock { Text = "HIGH", HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.White, FontSize = 10 };
        var midLabel = new TextBlock { Text = "MID", HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.White, FontSize = 10 };
        var lowLabel = new TextBlock { Text = "LOW", HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.White, FontSize = 10 };

        Grid.SetColumn(highLabel, 0);
        Grid.SetColumn(midLabel, 1);
        Grid.SetColumn(lowLabel, 2);
        Grid.SetRow(highLabel, 0);
        Grid.SetRow(midLabel, 0);
        Grid.SetRow(lowLabel, 0);

        // Sliders
        var highSlider = new Slider 
        { 
            Orientation = Orientation.Vertical, 
            Minimum = 0, 
            Maximum = 1, 
            Value = 0.5,
            Height = 80,
            Margin = new Thickness(5)
        };
        var midSlider = new Slider 
        { 
            Orientation = Orientation.Vertical, 
            Minimum = 0, 
            Maximum = 1, 
            Value = 0.5,
            Height = 80,
            Margin = new Thickness(5)
        };
        var lowSlider = new Slider 
        { 
            Orientation = Orientation.Vertical, 
            Minimum = 0, 
            Maximum = 1, 
            Value = 0.5,
            Height = 80,
            Margin = new Thickness(5)
        };

        // Bind sliders to properties
        highSlider.Bind(Slider.ValueProperty, this.GetObservable(HighProperty));
        midSlider.Bind(Slider.ValueProperty, this.GetObservable(MidProperty));
        lowSlider.Bind(Slider.ValueProperty, this.GetObservable(LowProperty));

        Grid.SetColumn(highSlider, 0);
        Grid.SetColumn(midSlider, 1);
        Grid.SetColumn(lowSlider, 2);
        Grid.SetRow(highSlider, 1);
        Grid.SetRow(midSlider, 1);
        Grid.SetRow(lowSlider, 1);

        grid.Children.Add(highLabel);
        grid.Children.Add(midLabel);
        grid.Children.Add(lowLabel);
        grid.Children.Add(highSlider);
        grid.Children.Add(midSlider);
        grid.Children.Add(lowSlider);

        Content = grid;
        Width = 120;
        Height = 100;
    }
}
