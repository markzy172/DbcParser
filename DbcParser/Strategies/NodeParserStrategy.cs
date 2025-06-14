using DbcParser.Parser;

namespace DbcParser.Strategies;

public class NodeParserStrategy : ILineParserStrategy
{
    public bool CanParse(string line) => line.StartsWith("BU_");

    public void Parse(string line, DbcParsingContext context)
    {
        var parts = line.Substring(4).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        context.File.Nodes.AddRange(parts);
    }
}