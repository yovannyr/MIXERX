using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace MIXERX.UI.Controls;

public class WaveformControl : Control
{
    public static readonly StyledProperty<float[]?> WaveformDataProperty =
        AvaloniaProperty.Register<WaveformControl, float[]?>(nameof(WaveformData));

    public static readonly StyledProperty<double> PositionProperty =
        AvaloniaProperty.Register<WaveformControl, double>(nameof(Position));

    public static readonly StyledProperty<bool> IsPlayingProperty =
        AvaloniaProperty.Register<WaveformControl, bool>(nameof(IsPlaying));

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

    static WaveformControl()
    {
        AffectsRender<WaveformControl>(WaveformDataProperty, PositionProperty, IsPlayingProperty);
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
        DrawPlayhead(context);
    }

    private void DrawPlaceholder(DrawingContext context)
    {
        var rect = new Rect(0, 0, Bounds.Width, Bounds.Height);
        var brush = new SolidColorBrush(Color.FromRgb(40, 40, 40));
        context.FillRectangle(brush, rect);

        var textBrush = new SolidColorBrush(Color.FromRgb(120, 120, 120));
        var text = new FormattedText("Load Track", System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, Typeface.Default, 14, textBrush);
        
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

        var samplesPerPixel = WaveformData.Length / width;
        
        for (int x = 0; x < width; x++)
        {
            var sampleIndex = (int)(x * samplesPerPixel);
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

// Enhanced Spinning Vinyl Animation Control with Album Cover
public class VinylControl : Control
{
    public static readonly StyledProperty<bool> IsPlayingProperty =
        AvaloniaProperty.Register<VinylControl, bool>(nameof(IsPlaying));

    public static readonly StyledProperty<double> RpmProperty =
        AvaloniaProperty.Register<VinylControl, double>(nameof(Rpm), 33.3);

    public static readonly StyledProperty<Bitmap?> AlbumCoverProperty =
        AvaloniaProperty.Register<VinylControl, Bitmap?>(nameof(AlbumCover));

    private double _rotation = 0;
    private DateTime _lastUpdate = DateTime.Now;

    public bool IsPlaying
    {
        get => GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }

    public double Rpm
    {
        get => GetValue(RpmProperty);
        set => SetValue(RpmProperty, value);
    }

    public Bitmap? AlbumCover
    {
        get => GetValue(AlbumCoverProperty);
        set => SetValue(AlbumCoverProperty, value);
    }

    static VinylControl()
    {
        AffectsRender<VinylControl>(IsPlayingProperty, RpmProperty, AlbumCoverProperty);
    }

    public VinylControl()
    {
        // Animation timer
        var timer = new System.Timers.Timer(16); // ~60 FPS
        timer.Elapsed += (s, e) => UpdateRotation();
        timer.Start();
    }

    private void UpdateRotation()
    {
        if (!IsPlaying) return;

        var now = DateTime.Now;
        var deltaTime = (now - _lastUpdate).TotalSeconds;
        _lastUpdate = now;

        // Calculate rotation based on RPM
        var degreesPerSecond = (Rpm / 60.0) * 360.0;
        _rotation += degreesPerSecond * deltaTime;
        _rotation %= 360;

        Avalonia.Threading.Dispatcher.UIThread.Post(InvalidateVisual);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
        var radius = Math.Min(Bounds.Width, Bounds.Height) / 2 - 5;

        // Vinyl record background (black)
        var vinylBrush = new SolidColorBrush(Color.FromRgb(20, 20, 20));
        var vinylRect = new Rect(center.X - radius, center.Y - radius, radius * 2, radius * 2);
        context.FillRectangle(vinylBrush, vinylRect);

        // Album cover or label in center
        var coverRadius = radius * 0.6;
        var coverRect = new Rect(center.X - coverRadius, center.Y - coverRadius, coverRadius * 2, coverRadius * 2);
        
        if (AlbumCover != null)
        {
            // Draw album cover (simplified - no rotation for now)
            context.DrawImage(AlbumCover, coverRect);
        }
        else
        {
            // Default label when no album cover
            var labelBrush = IsPlaying 
                ? new SolidColorBrush(Color.FromRgb(255, 0, 0)) // Red when playing
                : new SolidColorBrush(Color.FromRgb(100, 100, 100)); // Gray when stopped
            
            context.FillRectangle(labelBrush, coverRect);

            // MIXERX logo text
            var textBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            var text = new FormattedText("MIXERX", System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, Typeface.Default, 12, textBrush);
            
            var textPos = new Point(
                center.X - text.Width / 2,
                center.Y - text.Height / 2);
            
            context.DrawText(text, textPos);
        }

        // Center hole (simplified)
        var holeRadius = radius * 0.08;
        var holeBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        var holeRect = new Rect(center.X - holeRadius, center.Y - holeRadius, holeRadius * 2, holeRadius * 2);
        context.FillRectangle(holeBrush, holeRect);
    }
}
