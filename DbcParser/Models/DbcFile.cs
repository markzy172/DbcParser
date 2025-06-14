namespace DbcParser.Models;

public class DbcFile
{
    public List<string> Nodes { get; set; } = new();
    public List<DbcMessage> Messages { get; set; } = new();
}