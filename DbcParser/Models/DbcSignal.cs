namespace DbcParser.Models;

public class DbcSignal
{
    public string Name { get; set; } = string.Empty;
    public int StartBit { get; set; }
    public int Length { get; set; }
    public bool IsLittleEndian { get; set; }
    public bool IsSigned { get; set; }
    public double Factor { get; set; }
    public double Offset { get; set; }
    public double Minimum { get; set; }
    public double Maximum { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Receiver { get; set; } = string.Empty;
    public List<DbcAttribute> Attributes { get; set; } = new();


    public string AttributesSummary =>
    Attributes != null && Attributes.Count > 0
        ? string.Join(", ", Attributes.Select(a => $"{a.Name}={a.Value}"))
        : string.Empty;
}