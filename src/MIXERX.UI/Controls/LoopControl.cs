using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace MIXERX.UI.Controls;

public class LoopControl : Control
{
    public static readonly StyledProperty<bool> IsLoopingProperty =
        AvaloniaProperty.Register<LoopControl, bool>(nameof(IsLooping));

    public static readonly StyledProperty<int> LoopLengthBeatsProperty =
        AvaloniaProperty.Register<LoopControl, int>(nameof(LoopLengthBeats));

    public static readonly StyledProperty<float> LoopProgressProperty =
        AvaloniaProperty.Register<LoopControl, float>(nameof(LoopProgress));

    public bool IsLooping
    {
        get => GetValue(IsLoopingProperty);
        set => SetValue(IsLoopingProperty, value);
    }

    public int LoopLengthBeats
    {
        get => GetValue(LoopLengthBeatsProperty);
        set => SetValue(LoopLengthBeatsProperty, value);
    }

    public float LoopProgress
    {
        get => GetValue(LoopProgressProperty);
        set => SetValue(LoopProgressProperty, value);
    }

    static LoopControl()
    {
        AffectsRender<LoopControl>(IsLoopingProperty, LoopLengthBeatsProperty, LoopProgressProperty);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var rect = new Rect(0, 0, Bounds.Width, Bounds.Height);
        
        // Background
        var bgBrush = IsLooping 
            ? new SolidColorBrush(Color.FromRgb(255, 100, 0)) // Orange when looping
            : new SolidColorBrush(Color.FromRgb(60, 60, 60));  // Gray when not looping
        
        context.FillRectangle(bgBrush, rect);

        // Progress bar
        if (IsLooping && LoopProgress > 0)
        {
            var progressWidth = Bounds.Width * LoopProgress;
            var progressRect = new Rect(0, 0, progressWidth, Bounds.Height);
            var progressBrush = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)); // Semi-transparent white
            context.FillRectangle(progressBrush, progressRect);
        }

        // Loop length text
        var textBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        var text = IsLooping ? $"{LoopLengthBeats}" : "LOOP";
        var formattedText = new FormattedText(text, System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, Typeface.Default, 12, textBrush);
        
        var textPos = new Point(
            (Bounds.Width - formattedText.Width) / 2,
            (Bounds.Height - formattedText.Height) / 2);
        
        context.DrawText(formattedText, textPos);

        // Border
        var borderBrush = IsLooping 
            ? new SolidColorBrush(Color.FromRgb(255, 150, 0))
            : new SolidColorBrush(Color.FromRgb(100, 100, 100));
        var pen = new Pen(borderBrush, 1);
        context.DrawRectangle(null, pen, rect);
    }
}
