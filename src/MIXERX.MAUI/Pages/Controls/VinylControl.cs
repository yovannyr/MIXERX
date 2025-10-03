using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace MIXERX.UI.Controls;

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
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(16) };

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

        IsPlayingProperty.Changed.AddClassHandler<VinylControl>((ctrl, _) => ctrl.HandleIsPlayingChanged());
    }

    public VinylControl()
    {
        _timer.Tick += (_, _) => UpdateRotation();
    }

    private void HandleIsPlayingChanged()
    {
        if (!IsAttachedToVisualTree)
            return;

        if (IsPlaying)
        {
            _lastUpdate = DateTime.Now;
            if (!_timer.IsEnabled)
                _timer.Start();
        }
        else
        {
            _timer.Stop();
        }
    }

    private void UpdateRotation()
    {
        if (!IsPlaying)
            return;

        var now = DateTime.Now;
        var deltaTime = (now - _lastUpdate).TotalSeconds;
        _lastUpdate = now;

        var degreesPerSecond = (Rpm / 60.0) * 360.0;
        _rotation = (_rotation + degreesPerSecond * deltaTime) % 360;

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
        var radius = Math.Min(Bounds.Width, Bounds.Height) / 2 - 5;

        var vinylBrush = new SolidColorBrush(Color.FromRgb(20, 20, 20));
        var vinylRect = new Rect(center.X - radius, center.Y - radius, radius * 2, radius * 2);
        context.FillRectangle(vinylBrush, vinylRect);

        var coverRadius = radius * 0.6;
        if (AlbumCover != null)
        {
            var coverRect = new Rect(center.X - coverRadius, center.Y - coverRadius, coverRadius * 2, coverRadius * 2);
            context.DrawImage(AlbumCover, coverRect);
        }
        else
        {
            var labelBrush = new SolidColorBrush(Color.FromRgb(100, 100, 100));
            var labelRect = new Rect(center.X - coverRadius, center.Y - coverRadius, coverRadius * 2, coverRadius * 2);
            context.FillRectangle(labelBrush, labelRect);
        }
    }

    private bool IsAttachedToVisualTree => VisualRoot != null;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (IsPlaying && !_timer.IsEnabled)
        {
            _lastUpdate = DateTime.Now;
            _timer.Start();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _timer.Stop();
        base.OnDetachedFromVisualTree(e);
    }
}
