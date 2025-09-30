namespace MIXERX.Engine.AI;

public class HarmonicMixingEngine
{
    // Camelot Wheel mapping for harmonic mixing
    private readonly Dictionary<string, CamelotKey> _camelotWheel = new()
    {
        // Major keys (outer wheel)
        ["Cmaj"] = new("8B", "C", "major"),
        ["Gmaj"] = new("9B", "G", "major"),
        ["Dmaj"] = new("10B", "D", "major"),
        ["Amaj"] = new("11B", "A", "major"),
        ["Emaj"] = new("12B", "E", "major"),
        ["Bmaj"] = new("1B", "B", "major"),
        ["F#maj"] = new("2B", "F#", "major"),
        ["C#maj"] = new("3B", "C#", "major"),
        ["G#maj"] = new("4B", "G#", "major"),
        ["D#maj"] = new("5B", "D#", "major"),
        ["A#maj"] = new("6B", "A#", "major"),
        ["Fmaj"] = new("7B", "F", "major"),
        
        // Minor keys (inner wheel)
        ["Amin"] = new("8A", "A", "minor"),
        ["Emin"] = new("9A", "E", "minor"),
        ["Bmin"] = new("10A", "B", "minor"),
        ["F#min"] = new("11A", "F#", "minor"),
        ["C#min"] = new("12A", "C#", "minor"),
        ["G#min"] = new("1A", "G#", "minor"),
        ["D#min"] = new("2A", "D#", "minor"),
        ["A#min"] = new("3A", "A#", "minor"),
        ["Fmin"] = new("4A", "F", "minor"),
        ["Cmin"] = new("5A", "C", "minor"),
        ["Gmin"] = new("6A", "G", "minor"),
        ["Dmin"] = new("7A", "D", "minor")
    };

    public HarmonicMixingRecommendation GetHarmonicRecommendations(string currentKey, IEnumerable<string> availableKeys)
    {
        if (!_camelotWheel.TryGetValue(currentKey, out var currentCamelot))
        {
            return new HarmonicMixingRecommendation
            {
                CurrentKey = currentKey,
                Error = "Unknown key format"
            };
        }

        var recommendations = new List<KeyRecommendation>();

        foreach (var key in availableKeys.Where(k => k != currentKey))
        {
            if (_camelotWheel.TryGetValue(key, out var targetCamelot))
            {
                var compatibility = CalculateHarmonicCompatibility(currentCamelot, targetCamelot);
                if (compatibility.Score > 0.5f)
                {
                    recommendations.Add(new KeyRecommendation
                    {
                        Key = key,
                        CamelotCode = targetCamelot.Code,
                        CompatibilityScore = compatibility.Score,
                        MixingRule = compatibility.Rule,
                        Difficulty = compatibility.Difficulty,
                        Description = compatibility.Description
                    });
                }
            }
        }

        return new HarmonicMixingRecommendation
        {
            CurrentKey = currentKey,
            CurrentCamelotCode = currentCamelot.Code,
            Recommendations = recommendations.OrderByDescending(r => r.CompatibilityScore).ToArray(),
            MixingTips = GenerateMixingTips(currentCamelot, recommendations)
        };
    }

    private HarmonicCompatibility CalculateHarmonicCompatibility(CamelotKey from, CamelotKey to)
    {
        var fromNumber = ExtractNumber(from.Code);
        var toNumber = ExtractNumber(to.Code);
        var fromLetter = ExtractLetter(from.Code);
        var toLetter = ExtractLetter(to.Code);

        // Perfect match (same key)
        if (from.Code == to.Code)
        {
            return new HarmonicCompatibility
            {
                Score = 1.0f,
                Rule = "Same Key",
                Difficulty = MixingDifficulty.Beginner,
                Description = "Perfect harmonic match - identical key"
            };
        }

        // Energy boost/drop (same number, different letter)
        if (fromNumber == toNumber && fromLetter != toLetter)
        {
            var isEnergyBoost = fromLetter == 'A' && toLetter == 'B';
            return new HarmonicCompatibility
            {
                Score = 0.95f,
                Rule = isEnergyBoost ? "Energy Boost" : "Energy Drop",
                Difficulty = MixingDifficulty.Beginner,
                Description = isEnergyBoost 
                    ? "Perfect energy boost - minor to major" 
                    : "Perfect energy drop - major to minor"
            };
        }

        // Adjacent keys (+1/-1)
        var numberDiff = Math.Abs(fromNumber - toNumber);
        if (numberDiff == 1 || numberDiff == 11) // Handle wrap-around (12->1)
        {
            if (fromLetter == toLetter)
            {
                return new HarmonicCompatibility
                {
                    Score = 0.9f,
                    Rule = "Adjacent Key",
                    Difficulty = MixingDifficulty.Beginner,
                    Description = "Smooth transition - adjacent key on Camelot wheel"
                };
            }
        }

        // Dominant/Subdominant (+7/-7 or +5/-5)
        if (numberDiff == 7 || numberDiff == 5)
        {
            if (fromLetter == toLetter)
            {
                var isDominant = (toNumber - fromNumber + 12) % 12 == 7;
                return new HarmonicCompatibility
                {
                    Score = 0.85f,
                    Rule = isDominant ? "Dominant" : "Subdominant",
                    Difficulty = MixingDifficulty.Intermediate,
                    Description = isDominant 
                        ? "Strong resolution - dominant relationship" 
                        : "Smooth progression - subdominant relationship"
                };
            }
        }

        // Relative keys (same root, different mode)
        if (from.Root == to.Root && from.Mode != to.Mode)
        {
            return new HarmonicCompatibility
            {
                Score = 0.8f,
                Rule = "Relative Key",
                Difficulty = MixingDifficulty.Intermediate,
                Description = "Relative major/minor - shares same notes"
            };
        }

        // Parallel keys (same letter, +2/-2)
        if (fromLetter == toLetter && (numberDiff == 2 || numberDiff == 10))
        {
            return new HarmonicCompatibility
            {
                Score = 0.75f,
                Rule = "Parallel Key",
                Difficulty = MixingDifficulty.Intermediate,
                Description = "Parallel movement - maintains mode character"
            };
        }

        // Tritone (opposite on wheel)
        if (numberDiff == 6)
        {
            return new HarmonicCompatibility
            {
                Score = 0.6f,
                Rule = "Tritone",
                Difficulty = MixingDifficulty.Advanced,
                Description = "Dramatic contrast - tritone relationship (use with caution)"
            };
        }

        // Other relationships
        if (numberDiff <= 3 || numberDiff >= 9)
        {
            return new HarmonicCompatibility
            {
                Score = 0.5f,
                Rule = "Distant Key",
                Difficulty = MixingDifficulty.Advanced,
                Description = "Challenging mix - requires careful transition"
            };
        }

        return new HarmonicCompatibility
        {
            Score = 0.3f,
            Rule = "Incompatible",
            Difficulty = MixingDifficulty.Expert,
            Description = "Difficult harmonic relationship - not recommended"
        };
    }

    private string[] GenerateMixingTips(CamelotKey currentKey, List<KeyRecommendation> recommendations)
    {
        var tips = new List<string>();

        // General tips based on current key
        if (currentKey.Mode == "minor")
        {
            tips.Add("From minor keys: Try energy boost to relative major for uplifting feel");
        }
        else
        {
            tips.Add("From major keys: Try energy drop to relative minor for deeper mood");
        }

        // Tips based on available recommendations
        var hasEnergyBoost = recommendations.Any(r => r.MixingRule == "Energy Boost");
        var hasAdjacent = recommendations.Any(r => r.MixingRule == "Adjacent Key");
        var hasDominant = recommendations.Any(r => r.MixingRule == "Dominant");

        if (hasEnergyBoost)
            tips.Add("Energy boost available - perfect for building crowd energy");
        
        if (hasAdjacent)
            tips.Add("Adjacent keys available - safest mixing option for beginners");
            
        if (hasDominant)
            tips.Add("Dominant relationship available - creates strong musical resolution");

        // Advanced tips
        tips.Add("Use EQ to emphasize compatible frequencies during transition");
        tips.Add("Consider using harmonic effects (reverb/delay) to smooth key changes");

        return tips.ToArray();
    }

    private int ExtractNumber(string camelotCode)
    {
        return int.Parse(camelotCode.Substring(0, camelotCode.Length - 1));
    }

    private char ExtractLetter(string camelotCode)
    {
        return camelotCode.Last();
    }

    public string[] GetKeyProgression(string startKey, int steps = 4)
    {
        if (!_camelotWheel.TryGetValue(startKey, out var current))
            return Array.Empty<string>();

        var progression = new List<string> { startKey };
        var currentCode = current.Code;

        for (int i = 0; i < steps - 1; i++)
        {
            // Move clockwise on the wheel (most common progression)
            var nextKey = GetNextKeyClockwise(currentCode);
            if (nextKey != null)
            {
                progression.Add(nextKey);
                currentCode = _camelotWheel[nextKey].Code;
            }
        }

        return progression.ToArray();
    }

    private string? GetNextKeyClockwise(string camelotCode)
    {
        var number = ExtractNumber(camelotCode);
        var letter = ExtractLetter(camelotCode);
        var nextNumber = (number % 12) + 1;
        var nextCode = $"{nextNumber}{letter}";

        return _camelotWheel.Values.FirstOrDefault(k => k.Code == nextCode)?.Key;
    }
}

// Supporting classes
public record CamelotKey(string Code, string Root, string Mode)
{
    public string Key => _camelotWheel.FirstOrDefault(kvp => kvp.Value == this).Key ?? "";
    private static readonly Dictionary<string, CamelotKey> _camelotWheel = new();
}

public class HarmonicCompatibility
{
    public float Score { get; set; }
    public string Rule { get; set; } = "";
    public MixingDifficulty Difficulty { get; set; }
    public string Description { get; set; } = "";
}

public class HarmonicMixingRecommendation
{
    public string CurrentKey { get; set; } = "";
    public string CurrentCamelotCode { get; set; } = "";
    public KeyRecommendation[] Recommendations { get; set; } = Array.Empty<KeyRecommendation>();
    public string[] MixingTips { get; set; } = Array.Empty<string>();
    public string? Error { get; set; }
}

public class KeyRecommendation
{
    public string Key { get; set; } = "";
    public string CamelotCode { get; set; } = "";
    public float CompatibilityScore { get; set; }
    public string MixingRule { get; set; } = "";
    public MixingDifficulty Difficulty { get; set; }
    public string Description { get; set; } = "";
}

public enum MixingDifficulty
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}
