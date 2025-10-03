namespace MIXERX.Core.Models;

public class SmartCrate
{
    public string Name { get; set; } = "New Smart Crate";
    public List<CrateRule> Rules { get; set; } = new();
    public CrateLogic Logic { get; set; } = CrateLogic.And;
}