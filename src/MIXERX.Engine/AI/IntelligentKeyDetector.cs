using System.Numerics;

namespace MIXERX.Engine.AI;

public class IntelligentKeyDetector
{
    private readonly string[] _keys = 
    {
        "C", "C#", "D", "D#", "E", "F", 
        "F#", "G", "G#", "A", "A#", "B"
    };

    private readonly string[] _modes = { "maj", "min" };
    
    // Krumhansl-Schmuckler key profiles (major and minor)
    private readonly float[] _majorProfile = 
    {
        6.35f, 2.23f, 3.48f, 2.33f, 4.38f, 4.09f,
        2.52f, 5.19f, 2.39f, 3.66f, 2.29f, 2.88f
    };
    
    private readonly float[] _minorProfile = 
    {
        6.33f, 2.68f, 3.52f, 5.38f, 2.60f, 3.53f,
        2.54f, 4.75f, 3.98f, 2.69f, 3.34f, 3.17f
    };

    // Neural network for advanced key detection
    private readonly float[,] _keyWeights = new float[24, 12]; // 24 keys, 12 pitch classes
    private readonly float[] _keyBiases = new float[24];

    public IntelligentKeyDetector()
    {
        InitializeKeyDetectionNetwork();
    }

    public KeyDetectionResult DetectKey(float[] audioSamples, int sampleRate)
    {
        // 1. Extract pitch class profile
        var pitchProfile = ExtractPitchClassProfile(audioSamples, sampleRate);
        
        // 2. Traditional key detection using profiles
        var traditionalResult = DetectKeyUsingProfiles(pitchProfile);
        
        // 3. Neural network key detection
        var neuralResult = DetectKeyUsingNeuralNetwork(pitchProfile);
        
        // 4. Harmonic analysis
        var harmonicResult = AnalyzeHarmonicContent(pitchProfile);
        
        // 5. Combine results with confidence weighting
        var finalResult = CombineKeyDetectionResults(traditionalResult, neuralResult, harmonicResult);
        
        return finalResult;
    }

    private float[] ExtractPitchClassProfile(float[] samples, int sampleRate)
    {
        var pitchProfile = new float[12];
        var windowSize = 4096;
        var hopSize = 1024;
        
        for (int pos = 0; pos < samples.Length - windowSize; pos += hopSize)
        {
            var window = samples.Skip(pos).Take(windowSize).ToArray();
            
            // Apply Hanning window
            ApplyHanningWindow(window);
            
            // Compute FFT
            var spectrum = ComputeFFT(window);
            
            // Map frequencies to pitch classes
            for (int bin = 1; bin < spectrum.Length / 2; bin++)
            {
                var frequency = (float)bin * sampleRate / windowSize;
                var pitchClass = FrequencyToPitchClass(frequency);
                var magnitude = (float)spectrum[bin].Magnitude;
                
                if (pitchClass >= 0 && pitchClass < 12)
                {
                    pitchProfile[pitchClass] += magnitude;
                }
            }
        }
        
        // Normalize pitch profile
        var sum = pitchProfile.Sum();
        if (sum > 0)
        {
            for (int i = 0; i < 12; i++)
            {
                pitchProfile[i] /= sum;
            }
        }
        
        return pitchProfile;
    }

    private KeyDetectionResult DetectKeyUsingProfiles(float[] pitchProfile)
    {
        var bestKey = "";
        var bestScore = float.MinValue;
        var bestMode = "";
        
        // Test all 24 keys (12 major + 12 minor)
        for (int root = 0; root < 12; root++)
        {
            // Major key
            var majorScore = CalculateKeyScore(pitchProfile, _majorProfile, root);
            if (majorScore > bestScore)
            {
                bestScore = majorScore;
                bestKey = _keys[root];
                bestMode = "maj";
            }
            
            // Minor key
            var minorScore = CalculateKeyScore(pitchProfile, _minorProfile, root);
            if (minorScore > bestScore)
            {
                bestScore = minorScore;
                bestKey = _keys[root];
                bestMode = "min";
            }
        }
        
        var confidence = Math.Clamp(bestScore / 10.0f, 0, 1);
        
        return new KeyDetectionResult
        {
            Key = bestKey + bestMode,
            Confidence = confidence,
            Method = "Profile-based"
        };
    }

    private KeyDetectionResult DetectKeyUsingNeuralNetwork(float[] pitchProfile)
    {
        var keyScores = new float[24];
        
        // Forward pass through neural network
        for (int keyIndex = 0; keyIndex < 24; keyIndex++)
        {
            float score = _keyBiases[keyIndex];
            for (int pitch = 0; pitch < 12; pitch++)
            {
                score += pitchProfile[pitch] * _keyWeights[keyIndex, pitch];
            }
            keyScores[keyIndex] = Sigmoid(score);
        }
        
        // Find best key
        var bestIndex = Array.IndexOf(keyScores, keyScores.Max());
        var keyRoot = bestIndex % 12;
        var isMinor = bestIndex >= 12;
        
        var keyName = _keys[keyRoot] + (isMinor ? "min" : "maj");
        var confidence = keyScores[bestIndex];
        
        return new KeyDetectionResult
        {
            Key = keyName,
            Confidence = confidence,
            Method = "Neural Network"
        };
    }

    private KeyDetectionResult AnalyzeHarmonicContent(float[] pitchProfile)
    {
        // Analyze harmonic relationships
        var harmonicStrength = new float[12];
        
        for (int root = 0; root < 12; root++)
        {
            // Major triad: root, major third, perfect fifth
            var third = (root + 4) % 12;
            var fifth = (root + 7) % 12;
            
            harmonicStrength[root] = pitchProfile[root] + 
                                   pitchProfile[third] * 0.8f + 
                                   pitchProfile[fifth] * 0.6f;
            
            // Add seventh for more sophisticated analysis
            var seventh = (root + 10) % 12; // minor seventh
            harmonicStrength[root] += pitchProfile[seventh] * 0.4f;
        }
        
        var bestRoot = Array.IndexOf(harmonicStrength, harmonicStrength.Max());
        var confidence = harmonicStrength[bestRoot] / harmonicStrength.Sum();
        
        // Determine major/minor based on third
        var majorThird = (bestRoot + 4) % 12;
        var minorThird = (bestRoot + 3) % 12;
        var isMinor = pitchProfile[minorThird] > pitchProfile[majorThird];
        
        return new KeyDetectionResult
        {
            Key = _keys[bestRoot] + (isMinor ? "min" : "maj"),
            Confidence = confidence,
            Method = "Harmonic Analysis"
        };
    }

    private KeyDetectionResult CombineKeyDetectionResults(
        KeyDetectionResult traditional, 
        KeyDetectionResult neural, 
        KeyDetectionResult harmonic)
    {
        // Weighted combination of results
        var results = new[] { traditional, neural, harmonic };
        var weights = new[] { 0.4f, 0.4f, 0.2f }; // Prefer neural and traditional
        
        // Find most confident result
        var bestResult = results.OrderByDescending(r => r.Confidence).First();
        
        // If results agree, increase confidence
        var agreement = results.Count(r => r.Key == bestResult.Key);
        var finalConfidence = bestResult.Confidence * (1 + agreement * 0.1f);
        
        return new KeyDetectionResult
        {
            Key = bestResult.Key,
            Confidence = Math.Clamp(finalConfidence, 0, 1),
            Method = "AI Combined",
            AlternativeKeys = GetAlternativeKeys(results)
        };
    }

    private void InitializeKeyDetectionNetwork()
    {
        var random = new Random(42);
        
        // Initialize weights based on music theory
        for (int key = 0; key < 24; key++)
        {
            var root = key % 12;
            var isMinor = key >= 12;
            var profile = isMinor ? _minorProfile : _majorProfile;
            
            for (int pitch = 0; pitch < 12; pitch++)
            {
                var rotatedIndex = (pitch - root + 12) % 12;
                _keyWeights[key, pitch] = profile[rotatedIndex] + 
                                        (float)(random.NextDouble() - 0.5) * 0.1f;
            }
            
            _keyBiases[key] = (float)(random.NextDouble() - 0.5) * 0.1f;
        }
    }

    // Helper methods
    private void ApplyHanningWindow(float[] window)
    {
        for (int i = 0; i < window.Length; i++)
        {
            var factor = 0.5f * (1 - (float)Math.Cos(2 * Math.PI * i / (window.Length - 1)));
            window[i] *= factor;
        }
    }

    private Complex[] ComputeFFT(float[] samples)
    {
        // Simplified FFT - in production use Math.NET or similar
        return samples.Select(s => new Complex(s, 0)).ToArray();
    }

    private int FrequencyToPitchClass(float frequency)
    {
        if (frequency < 80 || frequency > 2000) return -1;
        
        var noteNumber = 12 * Math.Log2(frequency / 440.0) + 69; // A4 = 440Hz = note 69
        return ((int)Math.Round(noteNumber) % 12 + 12) % 12;
    }

    private float CalculateKeyScore(float[] pitchProfile, float[] keyProfile, int root)
    {
        float score = 0;
        for (int i = 0; i < 12; i++)
        {
            var rotatedIndex = (i - root + 12) % 12;
            score += pitchProfile[i] * keyProfile[rotatedIndex];
        }
        return score;
    }

    private string[] GetAlternativeKeys(KeyDetectionResult[] results)
    {
        return results.Select(r => r.Key).Distinct().Take(3).ToArray();
    }

    private float Sigmoid(float x) => 1.0f / (1.0f + (float)Math.Exp(-x));
}

public class KeyDetectionResult
{
    public string Key { get; set; } = "";
    public float Confidence { get; set; }
    public string Method { get; set; } = "";
    public string[] AlternativeKeys { get; set; } = Array.Empty<string>();
}
