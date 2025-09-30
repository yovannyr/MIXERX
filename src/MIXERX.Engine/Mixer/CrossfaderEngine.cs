namespace MIXERX.Engine.Mixer;

public class CrossfaderEngine
{
    private float _position = 0.0f; // -1.0 (A) to +1.0 (B)
    private CrossfaderCurve _curve = CrossfaderCurve.Linear;
    private readonly Dictionary<int, CrossfaderAssignment> _deckAssignments = new();

    public CrossfaderEngine()
    {
        // Default assignments: Deck 1&3 = A, Deck 2&4 = B
        _deckAssignments[1] = CrossfaderAssignment.A;
        _deckAssignments[2] = CrossfaderAssignment.B;
        _deckAssignments[3] = CrossfaderAssignment.A;
        _deckAssignments[4] = CrossfaderAssignment.B;
    }

    public float Position
    {
        get => _position;
        set => _position = Math.Clamp(value, -1.0f, 1.0f);
    }

    public CrossfaderCurve Curve
    {
        get => _curve;
        set => _curve = value;
    }

    // Calculate volume for a deck based on crossfader position
    public float GetDeckVolume(int deckId)
    {
        if (!_deckAssignments.TryGetValue(deckId, out var assignment))
            return 1.0f; // Thru (not assigned to crossfader)

        return assignment switch
        {
            CrossfaderAssignment.A => CalculateVolumeA(),
            CrossfaderAssignment.B => CalculateVolumeB(),
            CrossfaderAssignment.Thru => 1.0f,
            _ => 1.0f
        };
    }

    // Set deck assignment to crossfader
    public void SetDeckAssignment(int deckId, CrossfaderAssignment assignment)
    {
        _deckAssignments[deckId] = assignment;
    }

    // Calculate volume for A side based on curve
    private float CalculateVolumeA()
    {
        return _curve switch
        {
            CrossfaderCurve.Linear => Math.Max(0, 1.0f - (_position + 1.0f) / 2.0f),
            CrossfaderCurve.Logarithmic => CalculateLogVolume(-_position),
            CrossfaderCurve.Sharp => _position <= 0 ? 1.0f : 0.0f,
            _ => Math.Max(0, 1.0f - (_position + 1.0f) / 2.0f)
        };
    }

    // Calculate volume for B side based on curve
    private float CalculateVolumeB()
    {
        return _curve switch
        {
            CrossfaderCurve.Linear => Math.Max(0, (_position + 1.0f) / 2.0f),
            CrossfaderCurve.Logarithmic => CalculateLogVolume(_position),
            CrossfaderCurve.Sharp => _position >= 0 ? 1.0f : 0.0f,
            _ => Math.Max(0, (_position + 1.0f) / 2.0f)
        };
    }

    // Logarithmic crossfader curve (more natural mixing)
    private static float CalculateLogVolume(float position)
    {
        if (position <= -1.0f) return 0.0f;
        if (position >= 1.0f) return 1.0f;
        
        // Logarithmic curve: more gradual in middle, sharper at ends
        var normalized = (position + 1.0f) / 2.0f; // 0 to 1
        return (float)Math.Pow(normalized, 0.5); // Square root curve
    }

    // Get crossfader info for UI
    public CrossfaderInfo GetCrossfaderInfo()
    {
        return new CrossfaderInfo
        {
            Position = _position,
            Curve = _curve,
            VolumeA = CalculateVolumeA(),
            VolumeB = CalculateVolumeB(),
            DeckAssignments = new Dictionary<int, CrossfaderAssignment>(_deckAssignments)
        };
    }
}

public enum CrossfaderCurve
{
    Linear,      // Smooth linear fade
    Logarithmic, // Natural mixing curve
    Sharp        // Hard cut (scratch style)
}

public enum CrossfaderAssignment
{
    A,    // Left side
    B,    // Right side  
    Thru  // Bypass crossfader
}

public record CrossfaderInfo
{
    public float Position { get; init; }
    public CrossfaderCurve Curve { get; init; }
    public float VolumeA { get; init; }
    public float VolumeB { get; init; }
    public Dictionary<int, CrossfaderAssignment> DeckAssignments { get; init; } = new();
}
