using MIXERX.Engine.Analysis;

namespace MIXERX.Engine.Sync;

public class SyncEngine
{
    private readonly Dictionary<int, DeckSyncInfo> _deckInfo = new();
    private int? _masterDeckId;

    public void RegisterDeck(int deckId, float bpm, string key)
    {
        _deckInfo[deckId] = new DeckSyncInfo
        {
            DeckId = deckId,
            Bpm = bpm,
            Key = key,
            Phase = 0,
            IsSynced = false
        };
    }

    public void SetMasterDeck(int deckId)
    {
        _masterDeckId = deckId;
        
        if (_deckInfo.TryGetValue(deckId, out var info))
        {
            info.IsMaster = true;
        }
    }

    public SyncResult CalculateSync(int slaveDeckId)
    {
        if (!_masterDeckId.HasValue || !_deckInfo.ContainsKey(_masterDeckId.Value))
            return new SyncResult { CanSync = false };

        var master = _deckInfo[_masterDeckId.Value];
        var slave = _deckInfo.GetValueOrDefault(slaveDeckId);
        
        if (slave == null)
            return new SyncResult { CanSync = false };

        // Calculate tempo adjustment needed
        var tempoRatio = master.Bpm / slave.Bpm;
        
        // Calculate phase alignment
        var phaseOffset = CalculatePhaseOffset(master, slave);
        
        // Check key compatibility
        var keyCompatible = AreKeysCompatible(master.Key, slave.Key);

        return new SyncResult
        {
            CanSync = true,
            TempoAdjustment = tempoRatio,
            PhaseOffset = phaseOffset,
            KeyCompatible = keyCompatible,
            RecommendedKey = keyCompatible ? slave.Key : GetCompatibleKey(master.Key)
        };
    }

    public void UpdateDeckPhase(int deckId, float phase)
    {
        if (_deckInfo.TryGetValue(deckId, out var info))
        {
            info.Phase = phase % 1.0f; // Keep phase between 0-1
        }
    }

    private float CalculatePhaseOffset(DeckSyncInfo master, DeckSyncInfo slave)
    {
        // Calculate how much to offset slave to align with master beat
        var phaseDiff = master.Phase - slave.Phase;
        
        // Normalize to [-0.5, 0.5] range
        while (phaseDiff > 0.5f) phaseDiff -= 1.0f;
        while (phaseDiff < -0.5f) phaseDiff += 1.0f;
        
        return phaseDiff;
    }

    private bool AreKeysCompatible(string key1, string key2)
    {
        // Simplified key compatibility (same key or relative major/minor)
        var compatibleKeys = GetCompatibleKeys(key1);
        return compatibleKeys.Contains(key2);
    }

    private string[] GetCompatibleKeys(string key)
    {
        // Simplified compatibility - same key, perfect fifth, relative minor/major
        return key switch
        {
            "C" => new[] { "C", "G", "Am", "Em" },
            "G" => new[] { "G", "D", "Em", "Bm" },
            "D" => new[] { "D", "A", "Bm", "F#m" },
            "A" => new[] { "A", "E", "F#m", "C#m" },
            "E" => new[] { "E", "B", "C#m", "G#m" },
            "F" => new[] { "F", "C", "Dm", "Am" },
            _ => new[] { key } // Default to same key only
        };
    }

    private string GetCompatibleKey(string masterKey)
    {
        var compatible = GetCompatibleKeys(masterKey);
        return compatible.Length > 1 ? compatible[1] : masterKey;
    }
}

public class DeckSyncInfo
{
    public int DeckId { get; set; }
    public float Bpm { get; set; }
    public string Key { get; set; } = "";
    public float Phase { get; set; } // Beat phase (0-1)
    public bool IsSynced { get; set; }
    public bool IsMaster { get; set; }
}

public class SyncResult
{
    public bool CanSync { get; set; }
    public float TempoAdjustment { get; set; } = 1.0f;
    public float PhaseOffset { get; set; }
    public bool KeyCompatible { get; set; }
    public string RecommendedKey { get; set; } = "";
}

// Beat Grid for precise sync
public class BeatGrid
{
    private readonly float _bpm;
    private readonly float _sampleRate;
    private readonly float _beatsPerSecond;

    public BeatGrid(float bpm, float sampleRate)
    {
        _bpm = bpm;
        _sampleRate = sampleRate;
        _beatsPerSecond = bpm / 60.0f;
    }

    public float GetBeatPhase(long samplePosition)
    {
        var timeInSeconds = samplePosition / _sampleRate;
        var beatPosition = timeInSeconds * _beatsPerSecond;
        return beatPosition % 1.0f; // Return fractional part (0-1)
    }

    public long GetNextBeatSample(long currentSample)
    {
        var currentPhase = GetBeatPhase(currentSample);
        var samplesPerBeat = _sampleRate / _beatsPerSecond;
        var samplesToNextBeat = (1.0f - currentPhase) * samplesPerBeat;
        return currentSample + (long)samplesToNextBeat;
    }

    public bool IsOnBeat(long samplePosition, float tolerance = 0.05f)
    {
        var phase = GetBeatPhase(samplePosition);
        return phase < tolerance || phase > (1.0f - tolerance);
    }
}
