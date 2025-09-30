using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace MIXERX.UI.Controls;

public class CrossfaderControl : Control
{
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<CrossfaderControl, double>(nameof(Value), 0.5);

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, Math.Clamp(value, 0.0, 1.0));
    }

    private bool _isDragging;
    private double _startY;

    static CrossfaderControl()
    {
        AffectsRender<CrossfaderControl>(ValueProperty);
    }

    public CrossfaderControl()
    {
        Width = 40;
        Height = 200;
        
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isDragging = true;
        _startY = e.GetPosition(this).Y;
        e.Pointer.Capture(this);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging) return;
        
        var currentY = e.GetPosition(this).Y;
        var deltaY = currentY - _startY;
        var newValue = Value + (deltaY / Height);
        
        Value = Math.Clamp(newValue, 0.0, 1.0);
        _startY = currentY;
        
        InvalidateVisual();
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        e.Pointer.Capture(null);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        
        // Draw crossfader track
        var trackRect = new Rect(15, 10, 10, Height - 20);
        context.FillRectangle(Brushes.Gray, trackRect);
        
        // Draw crossfader knob
        var knobY = 10 + (Height - 40) * (1.0 - Value);
        var knobRect = new Rect(5, knobY, 30, 20);
        context.FillRectangle(Brushes.White, knobRect);
        context.DrawRectangle(new Pen(Brushes.Black, 1), knobRect);
        
        // Draw position markers
        var textBrush = Brushes.White;
        var textA = new FormattedText("A", System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, Typeface.Default, 10, textBrush);
        var textB = new FormattedText("B", System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, Typeface.Default, 10, textBrush);
            
        context.DrawText(textA, new Point(18, 5));
        context.DrawText(textB, new Point(18, Height - 15));
    }
}
