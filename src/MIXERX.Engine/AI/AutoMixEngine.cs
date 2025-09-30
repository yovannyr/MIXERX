namespace MIXERX.Engine.AI;

public class AutoMixEngine
{
    private readonly List<TrackAnalysis> _trackDatabase = new();
    private readonly Dictionary<string, float> _transitionRules = new();

    public AutoMixEngine()
    {
        InitializeTransitionRules();
    }

    public AutoMixSuggestion GetMixSuggestion(TrackAnalysis currentTrack, IEnumerable<TrackAnalysis> availableTracks)
    {
        var candidates = availableTracks.Where(t => t.Id != currentTrack.Id).ToList();
        var scoredTracks = new List<(TrackAnalysis track, float score, MixTransition transition)>();

        foreach (var candidate in candidates)
        {
            var transition = AnalyzeTransition(currentTrack, candidate);
            var score = CalculateCompatibilityScore(currentTrack, candidate, transition);
            
            scoredTracks.Add((candidate, score, transition));
        }

        // Sort by compatibility score
        var bestMatches = scoredTracks
            .OrderByDescending(x => x.score)
            .Take(5)
            .ToList();

        return new AutoMixSuggestion
        {
            CurrentTrack = currentTrack,
            RecommendedTracks = bestMatches.Select(x => new TrackRecommendation
            {
                Track = x.track,
                CompatibilityScore = x.score,
                Transition = x.transition,
                Reason = GenerateRecommendationReason(currentTrack, x.track, x.transition)
            }).ToArray()
        };
    }

    private MixTransition AnalyzeTransition(TrackAnalysis from, TrackAnalysis to)
    {
        var transition = new MixTransition
        {
            FromTrack = from,
            ToTrack = to
        };

        // 1. BPM Analysis
        var bpmRatio = to.Bpm / from.Bpm;
        transition.BpmCompatibility = CalculateBpmCompatibility(from.Bpm, to.Bpm);
        transition.RequiredTempoAdjustment = bpmRatio;

        // 2. Key Analysis
        transition.KeyCompatibility = CalculateKeyCompatibility(from.Key, to.Key);
        transition.HarmonicRelationship = GetHarmonicRelationship(from.Key, to.Key);

        // 3. Energy Analysis
        transition.EnergyTransition = CalculateEnergyTransition(from.Energy, to.Energy);
        
        // 4. Optimal Mix Points
        transition.OptimalMixInPoint = FindOptimalMixInPoint(from, to);
        transition.OptimalMixOutPoint = FindOptimalMixOutPoint(from, to);
        
        // 5. Suggested Transition Type
        transition.TransitionType = DetermineTransitionType(from, to);
        transition.TransitionDuration = CalculateOptimalTransitionDuration(from, to);

        return transition;
    }

    private float CalculateCompatibilityScore(TrackAnalysis from, TrackAnalysis to, MixTransition transition)
    {
        var score = 0.0f;

        // BPM Compatibility (30% weight)
        score += transition.BpmCompatibility * 0.3f;

        // Key Compatibility (25% weight)
        score += transition.KeyCompatibility * 0.25f;

        // Energy Flow (20% weight)
        var energyScore = CalculateEnergyFlowScore(from.Energy, to.Energy);
        score += energyScore * 0.2f;

        // Genre Compatibility (15% weight)
        var genreScore = CalculateGenreCompatibility(from.Genre, to.Genre);
        score += genreScore * 0.15f;

        // Mood Compatibility (10% weight)
        var moodScore = CalculateMoodCompatibility(from.Mood, to.Mood);
        score += moodScore * 0.1f;

        // Bonus for perfect matches
        if (transition.BpmCompatibility > 0.9f && transition.KeyCompatibility > 0.8f)
        {
            score += 0.1f; // Perfect match bonus
        }

        return Math.Clamp(score, 0, 1);
    }

    private float CalculateBpmCompatibility(float bpm1, float bpm2)
    {
        var ratio = Math.Max(bpm1, bpm2) / Math.Min(bpm1, bpm2);
        
        // Perfect match
        if (Math.Abs(bpm1 - bpm2) < 2) return 1.0f;
        
        // Double/half tempo
        if (Math.Abs(ratio - 2.0f) < 0.1f) return 0.9f;
        
        // 3/2 ratio (common in electronic music)
        if (Math.Abs(ratio - 1.5f) < 0.1f) return 0.8f;
        
        // Within ±10 BPM
        if (Math.Abs(bpm1 - bpm2) < 10) return 0.7f;
        
        // Within ±20 BPM
        if (Math.Abs(bpm1 - bpm2) < 20) return 0.5f;
        
        return 0.2f; // Poor compatibility
    }

    private float CalculateKeyCompatibility(string key1, string key2)
    {
        if (key1 == key2) return 1.0f;

        var compatibleKeys = GetCompatibleKeys(key1);
        
        if (compatibleKeys.Contains(key2))
        {
            return GetKeyCompatibilityScore(key1, key2);
        }
        
        return 0.3f; // Not compatible but mixable with caution
    }

    private string[] GetCompatibleKeys(string key)
    {
        // Camelot Wheel / Circle of Fifths compatibility
        var keyMappings = new Dictionary<string, string[]>
        {
            ["Cmaj"] = new[] { "Gmaj", "Fmaj", "Amin", "Emin", "Dmin" },
            ["Gmaj"] = new[] { "Dmaj", "Cmaj", "Emin", "Bmin", "Amin" },
            ["Dmaj"] = new[] { "Amaj", "Gmaj", "Bmin", "F#min", "Emin" },
            ["Amaj"] = new[] { "Emaj", "Dmaj", "F#min", "C#min", "Bmin" },
            ["Emaj"] = new[] { "Bmaj", "Amaj", "C#min", "G#min", "F#min" },
            ["Bmaj"] = new[] { "F#maj", "Emaj", "G#min", "D#min", "C#min" },
            ["F#maj"] = new[] { "C#maj", "Bmaj", "D#min", "A#min", "G#min" },
            ["C#maj"] = new[] { "G#maj", "F#maj", "A#min", "Fmin", "D#min" },
            ["G#maj"] = new[] { "D#maj", "C#maj", "Fmin", "Cmin", "A#min" },
            ["D#maj"] = new[] { "A#maj", "G#maj", "Cmin", "Gmin", "Fmin" },
            ["A#maj"] = new[] { "Fmaj", "D#maj", "Gmin", "Dmin", "Cmin" },
            ["Fmaj"] = new[] { "Cmaj", "A#maj", "Dmin", "Amin", "Gmin" },
            // Minor keys
            ["Amin"] = new[] { "Emin", "Dmin", "Cmaj", "Gmaj", "Fmaj" },
            ["Emin"] = new[] { "Bmin", "Amin", "Gmaj", "Dmaj", "Cmaj" },
            // ... (continue for all minor keys)
        };

        return keyMappings.GetValueOrDefault(key, Array.Empty<string>());
    }

    private float GetKeyCompatibilityScore(string key1, string key2)
    {
        // Perfect fifth: 0.9
        // Relative major/minor: 0.8
        // Same key family: 0.7
        // Adjacent keys: 0.6
        
        if (IsRelativeMajorMinor(key1, key2)) return 0.8f;
        if (IsPerfectFifth(key1, key2)) return 0.9f;
        if (IsSameKeyFamily(key1, key2)) return 0.7f;
        
        return 0.6f; // Adjacent/compatible
    }

    private TimeSpan FindOptimalMixInPoint(TrackAnalysis from, TrackAnalysis to)
    {
        // Find best intro point in the incoming track
        // Look for low energy sections, breakdowns, or natural entry points
        
        var introDuration = TimeSpan.FromSeconds(16); // Default 16 seconds
        
        // Analyze track structure
        if (to.Structure?.Intro != null)
        {
            return to.Structure.Intro.End;
        }
        
        // Look for first breakdown or low-energy section
        var firstBreakdown = to.EnergyProfile?.FirstOrDefault(e => e.Energy < 0.3f);
        if (firstBreakdown != null)
        {
            return firstBreakdown.Timestamp;
        }
        
        return introDuration;
    }

    private TimeSpan FindOptimalMixOutPoint(TrackAnalysis from, TrackAnalysis to)
    {
        // Find best outro point in the outgoing track
        var totalDuration = from.Duration;
        var defaultOutro = totalDuration - TimeSpan.FromSeconds(32);
        
        // Look for natural outro or breakdown
        if (from.Structure?.Outro != null)
        {
            return from.Structure.Outro.Start;
        }
        
        // Find last high-energy section before end
        var lastSection = from.EnergyProfile?.LastOrDefault(e => e.Energy > 0.7f);
        if (lastSection != null)
        {
            return lastSection.Timestamp + TimeSpan.FromSeconds(16);
        }
        
        return defaultOutro;
    }

    private TransitionType DetermineTransitionType(TrackAnalysis from, TrackAnalysis to)
    {
        var energyDiff = to.Energy - from.Energy;
        var bpmDiff = Math.Abs(to.Bpm - from.Bpm);
        
        if (energyDiff > 0.3f) return TransitionType.EnergyBoost;
        if (energyDiff < -0.3f) return TransitionType.Breakdown;
        if (bpmDiff < 5) return TransitionType.Seamless;
        if (to.Genre != from.Genre) return TransitionType.GenreSwitch;
        
        return TransitionType.Standard;
    }

    private void InitializeTransitionRules()
    {
        // Initialize AI transition rules based on music theory and DJ best practices
        _transitionRules["same_key"] = 1.0f;
        _transitionRules["perfect_fifth"] = 0.9f;
        _transitionRules["relative_minor"] = 0.8f;
        _transitionRules["same_bpm"] = 1.0f;
        _transitionRules["double_bpm"] = 0.9f;
        _transitionRules["energy_flow_up"] = 0.8f;
        _transitionRules["energy_flow_down"] = 0.6f;
    }

    private string GenerateRecommendationReason(TrackAnalysis from, TrackAnalysis to, MixTransition transition)
    {
        var reasons = new List<string>();
        
        if (transition.BpmCompatibility > 0.9f)
            reasons.Add("Perfect BPM match");
        else if (transition.BpmCompatibility > 0.7f)
            reasons.Add("Good BPM compatibility");
            
        if (transition.KeyCompatibility > 0.8f)
            reasons.Add($"Harmonic match ({transition.HarmonicRelationship})");
            
        if (transition.EnergyTransition > 0.7f)
            reasons.Add("Smooth energy flow");
            
        return string.Join(", ", reasons);
    }

    // Helper methods
    private float CalculateEnergyTransition(float fromEnergy, float toEnergy) => 
        1.0f - Math.Abs(fromEnergy - toEnergy);
    
    private float CalculateEnergyFlowScore(float from, float to) =>
        to > from ? 0.8f : 0.6f; // Prefer energy increase
    
    private float CalculateGenreCompatibility(string genre1, string genre2) =>
        genre1 == genre2 ? 1.0f : GetGenreDistance(genre1, genre2);
    
    private float CalculateMoodCompatibility(string mood1, string mood2) =>
        mood1 == mood2 ? 1.0f : 0.7f;
    
    private float GetGenreDistance(string genre1, string genre2) => 0.5f; // Simplified
    private string GetHarmonicRelationship(string key1, string key2) => "Compatible";
    private bool IsRelativeMajorMinor(string key1, string key2) => false;
    private bool IsPerfectFifth(string key1, string key2) => false;
    private bool IsSameKeyFamily(string key1, string key2) => false;
    private TimeSpan CalculateOptimalTransitionDuration(TrackAnalysis from, TrackAnalysis to) => TimeSpan.FromSeconds(16);
}

// Supporting classes
public class AutoMixSuggestion
{
    public TrackAnalysis CurrentTrack { get; set; } = new();
    public TrackRecommendation[] RecommendedTracks { get; set; } = Array.Empty<TrackRecommendation>();
}

public class TrackRecommendation
{
    public TrackAnalysis Track { get; set; } = new();
    public float CompatibilityScore { get; set; }
    public MixTransition Transition { get; set; } = new();
    public string Reason { get; set; } = "";
}

public class MixTransition
{
    public TrackAnalysis FromTrack { get; set; } = new();
    public TrackAnalysis ToTrack { get; set; } = new();
    public float BpmCompatibility { get; set; }
    public float KeyCompatibility { get; set; }
    public float EnergyTransition { get; set; }
    public float RequiredTempoAdjustment { get; set; }
    public string HarmonicRelationship { get; set; } = "";
    public TimeSpan OptimalMixInPoint { get; set; }
    public TimeSpan OptimalMixOutPoint { get; set; }
    public TransitionType TransitionType { get; set; }
    public TimeSpan TransitionDuration { get; set; }
}

public class TrackAnalysis
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Artist { get; set; } = "";
    public float Bpm { get; set; }
    public string Key { get; set; } = "";
    public float Energy { get; set; }
    public string Genre { get; set; } = "";
    public string Mood { get; set; } = "";
    public TimeSpan Duration { get; set; }
    public TrackStructure? Structure { get; set; }
    public EnergyPoint[]? EnergyProfile { get; set; }
}

public class TrackStructure
{
    public Section? Intro { get; set; }
    public Section? Outro { get; set; }
    public Section[]? Verses { get; set; }
    public Section[]? Choruses { get; set; }
}

public class Section
{
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public string Type { get; set; } = "";
}

public class EnergyPoint
{
    public TimeSpan Timestamp { get; set; }
    public float Energy { get; set; }
}

public enum TransitionType
{
    Standard,
    Seamless,
    EnergyBoost,
    Breakdown,
    GenreSwitch
}
