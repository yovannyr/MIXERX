namespace MIXERX.Core.Models;

public struct AudioMetrics
{
    public float CpuUsage { get; set; }
    public int BufferUnderruns { get; set; }
    public long SamplesProcessed { get; set; }
    public double LatencyMs { get; set; }
}