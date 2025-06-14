using DbcParser.Parser;

namespace DbcParser.Strategies;

public interface ILineParserStrategy
{
    bool CanParse(string line);
    void Parse(string line, DbcParsingContext context);
}