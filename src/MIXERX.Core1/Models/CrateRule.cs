namespace MIXERX.Core.Models;

public class CrateRule
{
    public string Field { get; set; } = "BPM";
    public RuleOperator Operator { get; set; } = RuleOperator.Between;
    public string Value { get; set; } = "120-130";
}