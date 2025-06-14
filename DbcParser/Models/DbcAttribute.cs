namespace DbcParser.Models;

public class DbcAttribute
{
    public string Type { get; set; }
    public int MessageId { get; set; }
    public string SignalName { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}