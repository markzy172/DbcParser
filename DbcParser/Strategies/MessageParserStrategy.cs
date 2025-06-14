using System.Text.RegularExpressions;
using DbcParser.Models;
using DbcParser.Parser;

namespace DbcParser.Strategies;

public class MessageParserStrategy : ILineParserStrategy
{
    public bool CanParse(string line) => line.StartsWith("BO_");

    public void Parse(string line, DbcParsingContext context)
    {
        var match = Regex.Match(line, @"BO_\s+(\d+)\s+(\w+)\s*:\s*(\d+)\s+(\w+)");
        if (!match.Success) return;

        var message = new DbcMessage
        {
            Id = int.Parse(match.Groups[1].Value),
            Name = match.Groups[2].Value,
            Length = int.Parse(match.Groups[3].Value),
            Sender = match.Groups[4].Value
        };

        context.File.Messages.Add(message);
        context.CurrentMessage = message;
    }
}