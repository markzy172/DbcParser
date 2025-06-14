using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DbcParser.Parser;

class Program
{
    static void Main()
    {
        int fileCount = 1000;
        int signalsPerMessage = 20;
        var parser = new DbcFileParser();
        string tempDir = Path.Combine(AppContext.BaseDirectory, "MockDbcFiles");

        // 1. In-memory benchmark
        var dbcContents = new string[fileCount];
        for (int i = 0; i < fileCount; i++)
            dbcContents[i] = GetMockDbcContent(i, signalsPerMessage);

        var swInMemory = Stopwatch.StartNew();
        Parallel.ForEach(dbcContents, content =>
        {
            var result = parser.ParseFromString(content);
        });
        swInMemory.Stop();
        Console.WriteLine($"[In-Memory Parsing] Parsed {fileCount} DBCs in {swInMemory.ElapsedMilliseconds} ms");

        // 2. File + Parse benchmark
        if (!Directory.Exists(tempDir) || Directory.GetFiles(tempDir, "*.dbc").Length < fileCount)
        {
            Directory.CreateDirectory(tempDir);
            Console.WriteLine("Generating DBC files...");
            for (int i = 0; i < fileCount; i++)
            {
                string path = Path.Combine(tempDir, $"mock_{i}.dbc");
                File.WriteAllText(path, GetMockDbcContent(i, signalsPerMessage));
            }
        }
        else
        {
            Console.WriteLine("Using existing DBC files in temp directory.");
        }

        var files = Directory.GetFiles(tempDir, "*.dbc");

        var swFromFiles = Stopwatch.StartNew();
        Parallel.ForEach(files, file =>
        {
            var result = parser.Parse(file);
        });
        swFromFiles.Stop();

        Console.WriteLine($"[File Parsing] Parsed {fileCount} DBCs from disk in {swFromFiles.ElapsedMilliseconds} ms");
        Console.WriteLine("\nBenchmark Summary:");
        Console.WriteLine($"In-Memory: {swInMemory.ElapsedMilliseconds} ms");
        Console.WriteLine($"File + Parse: {swFromFiles.ElapsedMilliseconds} ms");
        Console.WriteLine($"Temp DBC files in: {tempDir}");
    }

    static string GetMockDbcContent(int index, int signalCount)
    {
        var header = @$"
VERSION ""1.0""
BU_: NODE_{index}
BO_ {1000 + index} MSG_{index}: 8 NODE_{index}
";

        var signals = "";
        for (int s = 0; s < signalCount; s++)
        {
            signals += @$" SG_ SIGNAL_{index}_{s} : {s * 8}|8@1+ (1,0) [0|255] ""unit"" NODE_{index}
";
        }

        var attributes = "";
        for (int s = 0; s < signalCount; s++)
        {
            attributes += @$"BA_ ""GenSigUnit"" SG_ {1000 + index} SIGNAL_{index}_{s} ""rpm"";
";
        }

        return header + signals + attributes;
    }
}
