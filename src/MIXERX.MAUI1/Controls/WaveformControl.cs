using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace MIXERX.MAUI.Controls;

public class WaveformControl : SKCanvasView
{
    public static readonly BindableProperty WaveformDataProperty =
        BindableProperty.Create(nameof(WaveformData), typeof(float[]), typeof(WaveformControl), null, propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty PositionProperty =
        BindableProperty.Create(nameof(Position), typeof(double), typeof(WaveformControl), 0.0, propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty IsPlayingProperty =
        BindableProperty.Create(nameof(IsPlaying), typeof(bool), typeof(WaveformControl), false, propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty DeckColorProperty =
        BindableProperty.Create(nameof(DeckColor), typeof(Color), typeof(WaveformControl), Colors.Green, propertyChanged: OnPropertyChanged);

    public float[]? WaveformData
    {
        get => (float[]?)GetValue(WaveformDataProperty);
        set => SetValue(WaveformDataProperty, value);
    }

    public double Position
    {
        get => (double)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }

    public Color DeckColor
    {
        get => (Color)GetValue(DeckColorProperty);
        set => SetValue(DeckColorProperty, value);
    }

    public WaveformControl()
    {
        PaintSurface += OnPaintSurface;
    }

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is WaveformControl control)
        {
            control.InvalidateSurface();
        }
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Black);

        if (WaveformData == null || WaveformData.Length == 0)
        {
            DrawPlaceholder(canvas, e.Info);
            return;
        }

        DrawWaveform(canvas, e.Info);
        DrawPlayhead(canvas, e.Info);
    }

    private void DrawPlaceholder(SKCanvas canvas, SKImageInfo info)
    {
        using var paint = new SKPaint
        {
            Color = SKColors.Gray,
            TextSize = 16,
            IsAntialias = true,
            TextAlign = SKTextAlign.Center
        };

        canvas.DrawText("LOAD TRACK", info.Width / 2, info.Height / 2, paint);
    }

    private void DrawWaveform(SKCanvas canvas, SKImageInfo info)
    {
        if (WaveformData == null) return;

        var width = info.Width;
        var height = info.Height;
        var centerY = height / 2f;

        // Convert MAUI Color to SKColor
        var deckColor = DeckColor.ToSKColor();
        
        using var paint = new SKPaint
        {
            Color = IsPlaying ? deckColor : SKColor.Parse("#6464FF"),
            StrokeWidth = 1,
            IsAntialias = true
        };

        var samplesPerPixel = Math.Max(1, WaveformData.Length / width);

        for (int x = 0; x < width && x * samplesPerPixel < WaveformData.Length; x++)
        {
            var sampleIndex = x * samplesPerPixel;
            var sample = WaveformData[sampleIndex];
            
            var waveHeight = Math.Abs(sample) * (height / 2f) * 0.9f;
            var topY = centerY - waveHeight;
            var bottomY = centerY + waveHeight;

            canvas.DrawLine(x, topY, x, bottomY, paint);
        }
    }

    private void DrawPlayhead(SKCanvas canvas, SKImageInfo info)
    {
        var playheadX = (float)(Position * info.Width);

        using var paint = new SKPaint
        {
            Color = SKColors.White,
            StrokeWidth = 2,
            IsAntialias = true
        };

        canvas.DrawLine(playheadX, 0, playheadX, info.Height, paint);
    }
}

public static class ColorExtensions
{
    public static SKColor ToSKColor(this Color color)
    {
        return new SKColor(
            (byte)(color.Red * 255),
            (byte)(color.Green * 255),
            (byte)(color.Blue * 255),
            (byte)(color.Alpha * 255)
        );
    }
}
