namespace MIXERX.Infrastructure.Services.Audio;

public class TimeStretchEngine
{
    private const int WindowSize = 2048;
    private const int HopSize = 512;
    private readonly float[] _window;

    public TimeStretchEngine()
    {
        _window = CreateHannWindow(WindowSize);
    }

    public float[] Stretch(float[] input, float ratio)
    {
        if (Math.Abs(ratio - 1.0f) < 0.01f)
            return input;

        var outputLength = (int)(input.Length / ratio);
        var output = new float[outputLength];
        
        var inputPos = 0.0f;
        var outputPos = 0;

        while (outputPos < outputLength - WindowSize)
        {
            var inputIndex = (int)inputPos;
            if (inputIndex + WindowSize >= input.Length)
                break;

            // Copy windowed segment
            for (int i = 0; i < WindowSize && outputPos + i < outputLength; i++)
            {
                if (inputIndex + i < input.Length)
                {
                    output[outputPos + i] += input[inputIndex + i] * _window[i];
                }
            }

            inputPos += HopSize * ratio;
            outputPos += HopSize;
        }

        // Normalize
        for (int i = 0; i < output.Length; i++)
        {
            output[i] *= 0.5f;
        }

        return output;
    }

    private static float[] CreateHannWindow(int size)
    {
        var window = new float[size];
        for (int i = 0; i < size; i++)
        {
            window[i] = 0.5f * (1 - MathF.Cos(2 * MathF.PI * i / (size - 1)));
        }
        return window;
    }
}
