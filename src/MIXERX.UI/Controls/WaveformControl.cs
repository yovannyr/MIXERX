using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace MIXERX.UI.Controls;

public class WaveformControl : Control
{
    public static readonly StyledProperty<float[]?> WaveformDataProperty =
        AvaloniaProperty.Register<WaveformControl, float[]?>(nameof(WaveformData));

    public static readonly StyledProperty<double> PositionProperty =
        AvaloniaProperty.Register<WaveformControl, double>(nameof(Position));

    public static readonly StyledProperty<bool> IsPlayingProperty =
        AvaloniaProperty.Register<WaveformControl, bool>(nameof(IsPlaying));

    public static readonly StyledProperty<double> ZoomProperty =
        AvaloniaProperty.Register<WaveformControl, double>(nameof(Zoom), 1.0);

    public float[]? WaveformData
    {
        get => GetValue(WaveformDataProperty);
        set => SetValue(WaveformDataProperty, value);
    }

    public double Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public bool IsPlaying
    {
        get => GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }

    public double Zoom
    {
        get => GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, Math.Max(0.1, value));
    }

    private readonly List<double> _cuePoints = new();
    private readonly List<double> _beatMarkers = new();

    static WaveformControl()
    {
        AffectsRender<WaveformControl>(WaveformDataProperty, PositionProperty, IsPlayingProperty, ZoomProperty);
    }

    public WaveformControl()
    {
        PointerPressed += OnPointerPressed;
        PointerWheelChanged += OnPointerWheelChanged;
        
        // Generate sample beat markers (every 0.5 seconds for 120 BPM)
        for (double i = 0; i < 10; i += 0.5)
        {
            _beatMarkers.Add(i);
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var clickX = e.GetPosition(this).X;
        var newPosition = (clickX / Bounds.Width);
        
        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            // Set cue point
            _cuePoints.Add(newPosition);
        }
        else
        {
            // Seek to position
            Position = newPosition;
        }
        
        InvalidateVisual();
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        var zoomFactor = e.Delta.Y > 0 ? 1.2 : 0.8;
        Zoom = Math.Clamp(Zoom * zoomFactor, 0.1, 10.0);
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (WaveformData == null || WaveformData.Length == 0)
        {
            DrawPlaceholder(context);
            return;
        }

        DrawWaveform(context);
        DrawBeatMarkers(context);
        DrawCuePoints(context);
        DrawPlayhead(context);
    }

    private void DrawPlaceholder(DrawingContext context)
    {
        var rect = new Rect(0, 0, Bounds.Width, Bounds.Height);
        var brush = new SolidColorBrush(Color.FromRgb(40, 40, 40));
        context.FillRectangle(brush, rect);

        var textBrush = new SolidColorBrush(Color.FromRgb(120, 120, 120));
        var text = new FormattedText("Load Track - Shift+Click for Cue, Scroll to Zoom", 
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, Typeface.Default, 12, textBrush);
        
        var textPos = new Point(
            (Bounds.Width - text.Width) / 2,
            (Bounds.Height - text.Height) / 2);
        
        context.DrawText(text, textPos);
    }

    private void DrawWaveform(DrawingContext context)
    {
        if (WaveformData == null) return;

        var width = Bounds.Width;
        var height = Bounds.Height;
        var centerY = height / 2;

        // Background
        var bgBrush = new SolidColorBrush(Color.FromRgb(20, 20, 20));
        context.FillRectangle(bgBrush, new Rect(0, 0, width, height));

        // Waveform
        var waveformBrush = IsPlaying 
            ? new SolidColorBrush(Color.FromRgb(0, 255, 100)) // Green when playing
            : new SolidColorBrush(Color.FromRgb(100, 100, 255)); // Blue when paused

        var samplesPerPixel = Math.Max(1, (int)(WaveformData.Length / (width * Zoom)));
        var startSample = (int)(Position * WaveformData.Length);
        
        for (int x = 0; x < width; x++)
        {
            var sampleIndex = startSample + (int)(x * samplesPerPixel);
            if (sampleIndex >= WaveformData.Length) break;

            var amplitude = Math.Abs(WaveformData[sampleIndex]);
            var waveHeight = amplitude * (height / 2) * 0.8; // 80% of available height

            // Draw positive and negative parts
            var topY = centerY - waveHeight;
            var bottomY = centerY + waveHeight;
            
            context.DrawLine(new Pen(waveformBrush, 1), 
                new Point(x, topY), new Point(x, bottomY));
        }
    }

    private void DrawBeatMarkers(DrawingContext context)
    {
        var beatPen = new Pen(new SolidColorBrush(Color.FromRgb(255, 255, 0)), 1);
        
        foreach (var beat in _beatMarkers)
        {
            var beatX = beat * Bounds.Width / 10.0; // Assuming 10 second view
            if (beatX >= 0 && beatX <= Bounds.Width)
            {
                context.DrawLine(beatPen, new Point(beatX, 0), new Point(beatX, Bounds.Height));
            }
        }
    }

    private void DrawCuePoints(DrawingContext context)
    {
        var cuePen = new Pen(new SolidColorBrush(Color.FromRgb(255, 0, 0)), 3);
        
        foreach (var cue in _cuePoints)
        {
            var cueX = cue * Bounds.Width;
            if (cueX >= 0 && cueX <= Bounds.Width)
            {
                context.DrawLine(cuePen, new Point(cueX, 0), new Point(cueX, Bounds.Height));
                
                // Draw simple cue marker rectangle instead of triangle
                var markerRect = new Rect(cueX - 3, 0, 6, 10);
                context.FillRectangle(new SolidColorBrush(Color.FromRgb(255, 0, 0)), markerRect);
            }
        }
    }

    private void DrawPlayhead(DrawingContext context)
    {
        var playheadX = Position * Bounds.Width;
        var playheadBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        var pen = new Pen(playheadBrush, 2);

        context.DrawLine(pen, 
            new Point(playheadX, 0), 
            new Point(playheadX, Bounds.Height));
    }
}

