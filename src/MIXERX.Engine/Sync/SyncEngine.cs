using MIXERX.Core;

namespace MIXERX.Engine.Sync;

public class SyncEngine
{
    private readonly Dictionary<int, DeckSyncInfo> _deckInfo = new();
    private int? _masterDeckId;
    private float _masterBpm = 120.0f;

    public void RegisterDeck(int deckId, float bpm, long samplePosition, int sampleRate)
    {
        _deckInfo[deckId] = new DeckSyncInfo
        {
            DeckId = deckId,
            Bpm = bpm,
            SamplePosition = samplePosition,
            SampleRate = sampleRate,
            IsSynced = false
        };

        // First deck becomes master
        if (_masterDeckId == null && bpm > 0)
        {
            SetMasterDeck(deckId);
        }
    }

    public void SetMasterDeck(int deckId)
    {
        if (_deckInfo.TryGetValue(deckId, out var info))
        {
            _masterDeckId = deckId;
            _masterBpm = info.Bpm;
            
            // Update all other decks to sync to this master
            foreach (var deck in _deckInfo.Values)
            {
                if (deck.DeckId != deckId)
                {
                    deck.IsSynced = true;
                }
            }
        }
    }

    public void SyncDeck(int deckId, bool enable)
    {
        if (_deckInfo.TryGetValue(deckId, out var info))
        {
            info.IsSynced = enable;
        }
    }

    public float GetSyncedTempo(int deckId)
    {
        if (!_deckInfo.TryGetValue(deckId, out var info) || !info.IsSynced || _masterDeckId == null)
        {
            return 1.0f; // No sync, normal tempo
        }

        if (info.Bpm <= 0 || _masterBpm <= 0)
        {
            return 1.0f;
        }

        // Calculate tempo adjustment to match master BPM
        return _masterBpm / info.Bpm;
    }

    public long GetQuantizedPosition(int deckId, long currentPosition)
    {
        if (!_deckInfo.TryGetValue(deckId, out var info) || !info.IsSynced || _masterDeckId == null)
        {
            return currentPosition; // No quantization
        }

        if (!_deckInfo.TryGetValue(_masterDeckId.Value, out var masterInfo))
        {
            return currentPosition;
        }

        // Calculate beat length in samples
        var beatLengthSamples = (long)(info.SampleRate * 60.0 / info.Bpm);
        
        // Quantize to nearest beat
        var beatNumber = currentPosition / beatLengthSamples;
        return beatNumber * beatLengthSamples;
    }

    public bool IsDeckSynced(int deckId)
    {
        return _deckInfo.TryGetValue(deckId, out var info) && info.IsSynced;
    }

    public int? GetMasterDeckId()
    {
        return _masterDeckId;
    }

    public float GetMasterBpm()
    {
        return _masterBpm;
    }

    public void UpdateDeckBpm(int deckId, float bpm)
    {
        if (_deckInfo.TryGetValue(deckId, out var info))
        {
            info.Bpm = bpm;
            
            // If this is the master deck, update master BPM
            if (_masterDeckId == deckId)
            {
                _masterBpm = bpm;
            }
        }
    }

    public void UpdateDeckPosition(int deckId, long samplePosition)
    {
        if (_deckInfo.TryGetValue(deckId, out var info))
        {
            info.SamplePosition = samplePosition;
        }
    }
}

public class DeckSyncInfo
{
    public int DeckId { get; set; }
    public float Bpm { get; set; }
    public long SamplePosition { get; set; }
    public int SampleRate { get; set; }
    public bool IsSynced { get; set; }
}
