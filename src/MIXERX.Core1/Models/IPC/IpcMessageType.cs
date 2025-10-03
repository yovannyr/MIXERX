namespace MIXERX.Core.Models.IPC;

public enum IpcMessageType
{
    LoadTrack,
    Play,
    Pause,
    SetTempo,
    SetPosition,
    SetEffectParameter,
    SetCue,
    SetLoop,
    ExitLoop,
    LoopStatus,
    BpmDetected,
    SetSync,
    GetStatus,
    StatusResponse,
    StartRecording,
    StopRecording,
    WaveformData
}