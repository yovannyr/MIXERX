using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using MIXERX.Engine.Cues;

namespace MIXERX.UI.Controls;

public class HotCueButton : Button
{
    public static readonly StyledProperty<int> CueNumberProperty =
        AvaloniaProperty.Register<HotCueButton, int>(nameof(CueNumber));

    public static readonly StyledProperty<bool> IsSetProperty =
        AvaloniaProperty.Register<HotCueButton, bool>(nameof(IsSet));

    public static readonly StyledProperty<HotCueColor> CueColorProperty =
        AvaloniaProperty.Register<HotCueButton, HotCueColor>(nameof(CueColor));

    public static readonly StyledProperty<string> CueLabelProperty =
        AvaloniaProperty.Register<HotCueButton, string>(nameof(CueLabel), "");

    public int CueNumber
    {
        get => GetValue(CueNumberProperty);
        set => SetValue(CueNumberProperty, value);
    }

    public new bool IsSet
    {
        get => GetValue(IsSetProperty);
        set => SetValue(IsSetProperty, value);
    }

    public HotCueColor CueColor
    {
        get => GetValue(CueColorProperty);
        set => SetValue(CueColorProperty, value);
    }

    public string CueLabel
    {
        get => GetValue(CueLabelProperty);
        set => SetValue(CueLabelProperty, value);
    }

    // Events for different actions
    public event EventHandler<int>? HotCueTriggered;
    public event EventHandler<int>? HotCueSet;
    public event EventHandler<int>? HotCueDeleted;

    static HotCueButton()
    {
        AffectsRender<HotCueButton>(IsSetProperty, CueColorProperty);
    }

    public HotCueButton()
    {
        Width = 40;
        Height = 30;
        HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        var point = e.GetCurrentPoint(this);
        
        if (point.Properties.IsRightButtonPressed)
        {
            // Right click: Delete cue
            if (IsSet)
            {
                HotCueDeleted?.Invoke(this, CueNumber);
            }
        }
        else if (point.Properties.IsLeftButtonPressed)
        {
            if (IsSet)
            {
                // Left click on set cue: Trigger
                HotCueTriggered?.Invoke(this, CueNumber);
            }
            else
            {
                // Left click on empty cue: Set cue
                HotCueSet?.Invoke(this, CueNumber);
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        var rect = new Rect(0, 0, Bounds.Width, Bounds.Height);
        
        // Background color based on cue state
        IBrush backgroundBrush;
        if (IsSet)
        {
            backgroundBrush = new SolidColorBrush(GetCueColorRgb(CueColor));
        }
        else
        {
            backgroundBrush = new SolidColorBrush(Color.FromRgb(60, 60, 60)); // Dark gray
        }

        context.FillRectangle(backgroundBrush, rect);

        // Border
        var borderColor = IsSet ? Color.FromRgb(255, 255, 255) : Color.FromRgb(100, 100, 100);
        var pen = new Pen(new SolidColorBrush(borderColor), 1);
        context.DrawRectangle(null, pen, rect);

        // Text
        var textColor = IsSet ? Color.FromRgb(0, 0, 0) : Color.FromRgb(200, 200, 200);
        var textBrush = new SolidColorBrush(textColor);
        
        var displayText = IsSet && !string.IsNullOrEmpty(CueLabel) ? CueLabel : CueNumber.ToString();
        var text = new FormattedText(displayText, System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, Typeface.Default, 10, textBrush);
        
        var textPos = new Point(
            (Bounds.Width - text.Width) / 2,
            (Bounds.Height - text.Height) / 2);
        
        context.DrawText(text, textPos);
    }

    private static Color GetCueColorRgb(HotCueColor cueColor)
    {
        return cueColor switch
        {
            HotCueColor.Red => Color.FromRgb(255, 100, 100),
            HotCueColor.Orange => Color.FromRgb(255, 165, 0),
            HotCueColor.Yellow => Color.FromRgb(255, 255, 100),
            HotCueColor.Green => Color.FromRgb(100, 255, 100),
            HotCueColor.Blue => Color.FromRgb(100, 150, 255),
            HotCueColor.Purple => Color.FromRgb(200, 100, 255),
            HotCueColor.Pink => Color.FromRgb(255, 100, 200),
            HotCueColor.White => Color.FromRgb(255, 255, 255),
            _ => Color.FromRgb(255, 255, 255)
        };
    }
}
