using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbcParser.Models;
using DbcParser.Parser;
using DbcParser.Strategies;
using System;

namespace DbcParser.Tests
{
    [TestClass]
    public class StrategyTests
    {
        [TestMethod]
        public void NodeParserStrategy_Should_Parse_Nodes()
        {
            var strategy = new NodeParserStrategy();
            var context = new DbcParsingContext();

            string line = "BU_: Vector__XXX ECU1 ECU2";

            Assert.IsTrue(strategy.CanParse(line));
            strategy.Parse(line, context);

            CollectionAssert.AreEquivalent(new[] { "Vector__XXX", "ECU1", "ECU2" }, context.File.Nodes);
        }

        [TestMethod]
        public void MessageParserStrategy_Should_Parse_Message()
        {
            var strategy = new MessageParserStrategy();
            var context = new DbcParsingContext();

            string line = "BO_ 1234 EngineData: 8 Vector__XXX";

            Assert.IsTrue(strategy.CanParse(line));
            strategy.Parse(line, context);

            var message = context.File.Messages.Single();
            Assert.AreEqual(1234, message.Id);
            Assert.AreEqual("EngineData", message.Name);
            Assert.AreEqual(8, message.Length);
            Assert.AreEqual("Vector__XXX", message.Sender);
        }

        [TestMethod]
        public void SignalParserStrategy_Should_Parse_Signal()
        {
            var strategy = new SignalParserStrategy();
            var context = new DbcParsingContext();

            var message = new DbcMessage { Id = 1, Name = "TestMsg" };
            context.File.Messages.Add(message);
            context.CurrentMessage = message;

            string line = @"SG_ EngineSpeed : 0|16@1+ (0.125,0) [0|8000] ""rpm"" Vector__XXX";

            Assert.IsTrue(strategy.CanParse(line));
            strategy.Parse(line, context);

            var signal = message.Signals.Single();
            Assert.AreEqual("EngineSpeed", signal.Name);
            Assert.AreEqual(0, signal.StartBit);
            Assert.AreEqual(16, signal.Length);
            Assert.IsTrue(signal.IsLittleEndian);
            Assert.IsFalse(signal.IsSigned);
            Assert.AreEqual(0.125, signal.Factor, 0.0001);
            Assert.AreEqual(0, signal.Offset, 0.0001);
            Assert.AreEqual(0, signal.Minimum, 0.0001);
            Assert.AreEqual(8000, signal.Maximum, 0.0001);
            Assert.AreEqual("rpm", signal.Unit);
            Assert.AreEqual("Vector__XXX", signal.Receiver);
        }

        [TestMethod]
        public void AttributeParserStrategy_Should_Parse_SignalAttribute()
        {
            var strategy = new AttributeParserStrategy();
            var context = new DbcParsingContext();

            var message = new DbcMessage { Id = 123 };
            var signal = new DbcSignal { Name = "EngineSpeed" };
            message.Signals.Add(signal);
            context.File.Messages.Add(message);

            string line = @"BA_ ""GenSigUnit"" SG_ 123 EngineSpeed ""rpm""";

            Assert.IsTrue(strategy.CanParse(line));
            strategy.Parse(line, context);

            var attr = signal.Attributes.FirstOrDefault(a => a.Name == "GenSigUnit");
            Assert.IsNotNull(attr);
            Assert.AreEqual("rpm", attr.Value);
        }

        // --- NEGATIVE TESTS ---

        [TestMethod]
        public void NodeParserStrategy_Should_Reject_Invalid_Lines()
        {
            var strategy = new NodeParserStrategy();
            Assert.IsFalse(strategy.CanParse("BO_ 1234 EngineData: 8 Vector__XXX"));
            Assert.IsFalse(strategy.CanParse("SG_ SignalName : 0|8@1+ (1,0) [0|255] \"unit\" Node"));
        }

        [TestMethod]
        public void MessageParserStrategy_Should_Reject_Invalid_Lines()
        {
            var strategy = new MessageParserStrategy();
            Assert.IsFalse(strategy.CanParse("BU_: ECU1 ECU2"));
            Assert.IsFalse(strategy.CanParse("SG_ SignalName : 0|8@1+ (1,0) [0|255] \"unit\" Node"));
        }

        [TestMethod]
        public void SignalParserStrategy_Should_Reject_Invalid_Lines()
        {
            var strategy = new SignalParserStrategy();
            Assert.IsFalse(strategy.CanParse("BO_ 1234 EngineData: 8 Vector__XXX"));
            Assert.IsFalse(strategy.CanParse("BU_: ECU1 ECU2"));
        }

        [TestMethod]
        public void AttributeParserStrategy_Should_Reject_Invalid_Lines()
        {
            var strategy = new AttributeParserStrategy();
            Assert.IsFalse(strategy.CanParse("SG_ SignalName : 0|8@1+ (1,0) [0|255] \"unit\" Node"));
            Assert.IsFalse(strategy.CanParse("BO_ 1234 EngineData: 8 Vector__XXX"));
        }

        // --- GRACEFUL HANDLING ---

        [TestMethod]
        public void SignalParserStrategy_Should_Skip_Malformed_Signal_Silently()
        {
            var strategy = new SignalParserStrategy();
            var context = new DbcParsingContext();

            var message = new DbcMessage { Id = 1 };
            context.File.Messages.Add(message);
            context.CurrentMessage = message;

            string badLine = @"SG_ BadSignal : @1+ (1,0) [0|255] ""unit"" NODE";

            Assert.IsTrue(strategy.CanParse(badLine));

            strategy.Parse(badLine, context);
            Assert.AreEqual(0, message.Signals.Count, "Malformed signal should not be added to the message.");
        }


        [TestMethod]
        public void AttributeParserStrategy_Should_Ignore_If_Signal_Not_Found()
        {
            var strategy = new AttributeParserStrategy();
            var context = new DbcParsingContext();

            var message = new DbcMessage { Id = 123 };
            context.File.Messages.Add(message);

            string line = @"BA_ ""GenSigUnit"" SG_ 123 EngineSpeed ""rpm""";

            Assert.IsTrue(strategy.CanParse(line));
            strategy.Parse(line, context);

            Assert.AreEqual(0, message.Signals.Count);
        }
    }
}
