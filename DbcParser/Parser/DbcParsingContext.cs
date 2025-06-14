using DbcParser.Models;

namespace DbcParser.Parser;

public class DbcParsingContext
{
    public DbcFile File { get; } = new();
    public DbcMessage? CurrentMessage { get; set; }
}