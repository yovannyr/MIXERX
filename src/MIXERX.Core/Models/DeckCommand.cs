namespace MIXERX.Core.Models;

public record DeckCommand(DeckCommandType Type, string? StringParam = null);