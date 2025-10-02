namespace MIXERX.MAUI.Models;

public enum RuleOperator
{
    Equals,
    NotEquals,
    Contains,
    GreaterThan,
    LessThan,
    Between
}

public enum CrateLogic
{
    And,
    Or
}

public class CrateRule
{
    public string Field { get; set; } = "BPM";
    public RuleOperator Operator { get; set; } = RuleOperator.Between;
    public string Value { get; set; } = "120-130";
}

public class SmartCrate
{
    public string Name { get; set; } = "New Smart Crate";
    public List<CrateRule> Rules { get; set; } = new();
    public CrateLogic Logic { get; set; } = CrateLogic.And;
}
