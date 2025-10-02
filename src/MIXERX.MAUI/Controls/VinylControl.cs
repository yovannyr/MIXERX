using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace MIXERX.MAUI.Controls;

public class VinylControl : SKCanvasView
{
    public static readonly BindableProperty RotationAngleProperty =
        BindableProperty.Create(nameof(RotationAngle), typeof(float), typeof(VinylControl), 0f, propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty DeckColorProperty =
        BindableProperty.Create(nameof(DeckColor), typeof(Color), typeof(VinylControl), Colors.Cyan, propertyChanged: OnPropertyChanged);

    public float RotationAngle
    {
        get => (float)GetValue(RotationAngleProperty);
        set => SetValue(RotationAngleProperty, value);
    }

    public Color DeckColor
    {
        get => (Color)GetValue(DeckColorProperty);
        set => SetValue(DeckColorProperty, value);
    }

    public VinylControl()
    {
        PaintSurface += OnPaintSurface;
    }

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is VinylControl control)
            control.InvalidateSurface();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Black);

        var info = e.Info;
        var centerX = info.Width / 2f;
        var centerY = info.Height / 2f;
        var radius = Math.Min(centerX, centerY) * 0.9f;

        // Vinyl disc
        using var vinylPaint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawCircle(centerX, centerY, radius, vinylPaint);

        // Grooves
        using var groovePaint = new SKPaint
        {
            Color = new SKColor(40, 40, 40),
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1
        };
        for (int i = 0; i < 20; i++)
        {
            var r = radius * (0.3f + i * 0.035f);
            canvas.DrawCircle(centerX, centerY, r, groovePaint);
        }

        // Center label
        var labelRadius = radius * 0.25f;
        using var labelPaint = new SKPaint
        {
            Color = DeckColor.ToSKColor(),
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawCircle(centerX, centerY, labelRadius, labelPaint);

        // Rotation indicator
        canvas.Save();
        canvas.RotateDegrees(RotationAngle, centerX, centerY);
        using var indicatorPaint = new SKPaint
        {
            Color = SKColors.White,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };
        canvas.DrawCircle(centerX, centerY - labelRadius * 0.7f, 3, indicatorPaint);
        canvas.Restore();
    }
}
