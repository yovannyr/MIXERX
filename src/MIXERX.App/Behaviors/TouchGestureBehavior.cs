namespace MIXERX.App.Behaviors;

public class TouchGestureBehavior : Behavior<View>
{
    public event EventHandler<SwipeDirection>? Swiped;
    public event EventHandler? DoubleTapped;
    public event EventHandler<PinchEventArgs>? Pinched;

    private View? _view;
    private DateTime _lastTap;

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);
        _view = bindable;

        var swipe = new SwipeGestureRecognizer();
        swipe.Swiped += OnSwiped;
        _view.GestureRecognizers.Add(swipe);

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        _view.GestureRecognizers.Add(tap);

        var pinch = new PinchGestureRecognizer();
        pinch.PinchUpdated += OnPinchUpdated;
        _view.GestureRecognizers.Add(pinch);
    }

    private void OnSwiped(object? sender, SwipedEventArgs e)
    {
        Swiped?.Invoke(this, e.Direction);
    }

    private void OnTapped(object? sender, EventArgs e)
    {
        if ((DateTime.Now - _lastTap).TotalMilliseconds < 300)
        {
            DoubleTapped?.Invoke(this, EventArgs.Empty);
        }
        _lastTap = DateTime.Now;
    }

    private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        Pinched?.Invoke(this, new PinchEventArgs(e.Scale));
    }
}

public class PinchEventArgs : EventArgs
{
    public double Scale { get; }
    public PinchEventArgs(double scale) => Scale = scale;
}
