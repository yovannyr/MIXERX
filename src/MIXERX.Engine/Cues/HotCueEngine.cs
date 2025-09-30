namespace MIXERX.Engine.Cues;

public class HotCueEngine
{
    private readonly Dictionary<int, HotCue> _hotCues = new();
    private const int MAX_HOT_CUES = 8;

    public HotCueEngine()
    {
        // Initialize 8 hot cue slots
        for (int i = 1; i <= MAX_HOT_CUES; i++)
        {
            _hotCues[i] = new HotCue(i);
        }
    }

    // Set hot cue at current position
    public void SetHotCue(int cueNumber, long samplePosition, string? label = null)
    {
        if (cueNumber < 1 || cueNumber > MAX_HOT_CUES) return;

        _hotCues[cueNumber] = new HotCue(cueNumber)
        {
            SamplePosition = samplePosition,
            Label = label ?? $"CUE {cueNumber}",
            IsSet = true,
            Color = GetCueColor(cueNumber)
        };
    }

    // Jump to hot cue position
    public long? TriggerHotCue(int cueNumber)
    {
        if (cueNumber < 1 || cueNumber > MAX_HOT_CUES) return null;

        var cue = _hotCues[cueNumber];
        return cue.IsSet ? cue.SamplePosition : null;
    }

    // Delete hot cue
    public void DeleteHotCue(int cueNumber)
    {
        if (cueNumber < 1 || cueNumber > MAX_HOT_CUES) return;

        _hotCues[cueNumber] = new HotCue(cueNumber);
    }

    // Get all hot cues for UI display
    public HotCue[] GetAllHotCues()
    {
        return _hotCues.Values.OrderBy(c => c.Number).ToArray();
    }

    // Clear all hot cues (when loading new track)
    public void ClearAllHotCues()
    {
        for (int i = 1; i <= MAX_HOT_CUES; i++)
        {
            _hotCues[i] = new HotCue(i);
        }
    }

    // Get cue color based on number
    private static HotCueColor GetCueColor(int cueNumber)
    {
        return cueNumber switch
        {
            1 => HotCueColor.Red,
            2 => HotCueColor.Orange,
            3 => HotCueColor.Yellow,
            4 => HotCueColor.Green,
            5 => HotCueColor.Blue,
            6 => HotCueColor.Purple,
            7 => HotCueColor.Pink,
            8 => HotCueColor.White,
            _ => HotCueColor.White
        };
    }

    // Save/Load hot cues to/from track metadata
    public void SaveHotCuesToTrack(string trackPath)
    {
        // In production, save to track metadata or database
        // For now, just store in memory per session
    }

    public void LoadHotCuesFromTrack(string trackPath)
    {
        // In production, load from track metadata or database
        // For now, start with empty cues
        ClearAllHotCues();
    }
}

public class HotCue
{
    public int Number { get; }
    public long SamplePosition { get; set; }
    public string Label { get; set; } = "";
    public bool IsSet { get; set; }
    public HotCueColor Color { get; set; } = HotCueColor.White;

    public HotCue(int number)
    {
        Number = number;
        Label = $"CUE {number}";
    }

    // Get time position for display
    public TimeSpan GetTimePosition(int sampleRate)
    {
        return TimeSpan.FromSeconds((double)SamplePosition / sampleRate);
    }
}

public enum HotCueColor
{
    Red,
    Orange, 
    Yellow,
    Green,
    Blue,
    Purple,
    Pink,
    White
}
