using DbcParser.Models;
using DbcParser.Strategies;

namespace DbcParser.Parser;

public class DbcFileParser
{
    private readonly List<ILineParserStrategy> _strategies;

    public DbcFileParser()
    {
        _strategies = new List<ILineParserStrategy>
        {
            new NodeParserStrategy(),
            new MessageParserStrategy(),
            new SignalParserStrategy(),
            new AttributeParserStrategy()
        };
    }

    public DbcFile Parse(string path)
    {
        var context = new DbcParsingContext();
        var lines = File.ReadLines(path);

        foreach (var line in lines)
        {
            foreach (var strategy in _strategies)
            {
                if (strategy.CanParse(line))
                {
                    strategy.Parse(line, context);
                    break;
                }
            }
        }

        return context.File;
    }

    public DbcFile ParseFromString(string dbcContent)
    {
        var context = new DbcParsingContext();
        using var reader = new StringReader(dbcContent);
        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            foreach (var strategy in _strategies)
            {
                if (strategy.CanParse(line))
                {
                    strategy.Parse(line, context);
                    break;
                }
            }
        }

        return context.File;
    }

}