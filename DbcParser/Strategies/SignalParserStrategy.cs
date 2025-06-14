using System.Text.RegularExpressions;
using DbcParser.Models;
using DbcParser.Parser;

namespace DbcParser.Strategies;

public class SignalParserStrategy : ILineParserStrategy
{
    public bool CanParse(string line) => line.TrimStart().StartsWith("SG_");

    public void Parse(string line, DbcParsingContext context)
    {
        if (context.CurrentMessage == null) return;

        var match = Regex.Match(line, @"SG_\s+(\w+)\s*:\s*(\d+)\|(\d+)@(\d)([+-])\s+\(([^,]+),([^)]+)\)\s+\[([^|]+)\|([^\]]+)\]\s+""([^""]*)""\s+(\w+)");
        if (!match.Success) return;

        var signal = new DbcSignal
        {
            Name = match.Groups[1].Value,
            StartBit = int.Parse(match.Groups[2].Value),
            Length = int.Parse(match.Groups[3].Value),
            IsLittleEndian = match.Groups[4].Value == "1",
            IsSigned = match.Groups[5].Value == "-",
            Factor = double.Parse(match.Groups[6].Value),
            Offset = double.Parse(match.Groups[7].Value),
            Minimum = double.Parse(match.Groups[8].Value),
            Maximum = double.Parse(match.Groups[9].Value),
            Unit = match.Groups[10].Value,
            Receiver = match.Groups[11].Value
        };

        context.CurrentMessage.Signals.Add(signal);
    }
}