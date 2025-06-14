namespace DbcParser.Models;

public class DbcMessage
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Length { get; set; }
    public string Sender { get; set; } = string.Empty;
    public List<DbcSignal> Signals { get; set; } = new();
    public List<DbcAttribute> Attributes { get; set; } = new();
}