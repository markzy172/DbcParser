using System.Linq;
using System.Text.RegularExpressions;
using DbcParser.Models;
using DbcParser.Parser;

namespace DbcParser.Strategies
{
    public class AttributeParserStrategy : ILineParserStrategy
    {
        public bool CanParse(string line) => line.StartsWith("BA_ ");

        public void Parse(string line, DbcParsingContext context)
        {
            var signalMatch = Regex.Match(line, @"BA_\s+""(?<attr>[^""]+)""\s+SG_\s+(?<msgId>\d+)\s+(?<sigName>\w+)\s+""(?<value>[^""]*)""");
            if (signalMatch.Success)
            {
                var attribute = new DbcAttribute
                {
                    Type = "SG_",
                    MessageId = int.Parse(signalMatch.Groups["msgId"].Value),
                    SignalName = signalMatch.Groups["sigName"].Value,
                    Name = signalMatch.Groups["attr"].Value,
                    Value = signalMatch.Groups["value"].Value
                };

                var message = context.File.Messages.FirstOrDefault(m => m.Id == attribute.MessageId);
                var signal = message?.Signals.FirstOrDefault(s => s.Name == attribute.SignalName);

                if (signal != null)
                {
                    signal.Attributes.Add(attribute);
                }

                return;
            }

            var messageMatch = Regex.Match(line, @"BA_\s+""(?<attr>[^""]+)""\s+BO_\s+(?<msgId>\d+)\s+""(?<value>[^""]*)""");
            if (messageMatch.Success)
            {
                var attribute = new DbcAttribute
                {
                    Type = "BO_",
                    MessageId = int.Parse(messageMatch.Groups["msgId"].Value),
                    SignalName = null,
                    Name = messageMatch.Groups["attr"].Value,
                    Value = messageMatch.Groups["value"].Value
                };

                var message = context.File.Messages.FirstOrDefault(m => m.Id == attribute.MessageId);
                if (message != null)
                {
                    message.Attributes.Add(attribute);
                }
            }
        }
    }
}
