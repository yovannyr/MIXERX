using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace MIXERX.UI.Controls;

public class CrossfaderControl : Control
{
    public static readonly StyledProperty<double> PositionProperty =
        AvaloniaProperty.Register<CrossfaderControl, double>(nameof(Position), 0.0);

    public static readonly StyledProperty<double> VolumeAProperty =
        AvaloniaProperty.Register<CrossfaderControl, double>(nameof(VolumeA), 0.5);

    public static readonly StyledProperty<double> VolumeBProperty =
        AvaloniaProperty.Register<CrossfaderControl, double>(nameof(VolumeB), 0.5);

    public double Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, Math.Clamp(value, -1.0, 1.0));
    }

    public double VolumeA
    {
        get => GetValue(VolumeAProperty);
        set => SetValue(VolumeAProperty, value);
    }

    public double VolumeB
    {
        get => GetValue(VolumeBProperty);
        set => SetValue(VolumeBProperty, value);
    }

    static CrossfaderControl()
    {
        AffectsRender<CrossfaderControl>(PositionProperty, VolumeAProperty, VolumeBProperty);
    }

    public CrossfaderControl()
    {
        Height = 40;
        MinWidth = 200;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var rect = new Rect(0, 0, Bounds.Width, Bounds.Height);
        
        // Background track
        var trackBrush = new SolidColorBrush(Color.FromRgb(40, 40, 40));
        var trackRect = new Rect(10, Bounds.Height / 2 - 3, Bounds.Width - 20, 6);
        context.FillRectangle(trackBrush, trackRect);

        // Volume indicators (A and B sides)
        DrawVolumeIndicator(context, VolumeA, true);  // Left side (A)
        DrawVolumeIndicator(context, VolumeB, false); // Right side (B)

        // Crossfader knob
        var knobX = 10 + (Bounds.Width - 20) * ((Position + 1.0) / 2.0);
        var knobRect = new Rect(knobX - 8, Bounds.Height / 2 - 8, 16, 16);
        
        var knobBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
        context.FillRectangle(knobBrush, knobRect);
        
        var knobBorder = new Pen(new SolidColorBrush(Color.FromRgb(100, 100, 100)), 1);
        context.DrawRectangle(null, knobBorder, knobRect);

        // Labels
        DrawLabel(context, "A", 5, Bounds.Height - 15);
        DrawLabel(context, "B", Bounds.Width - 15, Bounds.Height - 15);
    }

    private void DrawVolumeIndicator(DrawingContext context, double volume, bool isLeftSide)
    {
        var maxWidth = (Bounds.Width - 40) / 2; // Half width minus margins
        var width = maxWidth * volume;
        
        var x = isLeftSide ? 10 : Bounds.Width - 10 - width;
        var y = 5;
        var height = 8;
        
        var color = isLeftSide 
            ? Color.FromRgb(255, 100, 100) // Red for A
            : Color.FromRgb(100, 100, 255); // Blue for B
            
        var brush = new SolidColorBrush(color);
        var rect = new Rect(x, y, width, height);
        
        context.FillRectangle(brush, rect);
    }

    private void DrawLabel(DrawingContext context, string text, double x, double y)
    {
        var textBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200));
        var formattedText = new FormattedText(text, System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, Typeface.Default, 10, textBrush);
        
        context.DrawText(formattedText, new Point(x, y));
    }
}
