using DbcParser.Parser;
using DbcParser.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;

namespace DbcParser.Gui.Services;

public static class DbcFileLoader
{
    public static List<DbcFile> LoadFiles()
    {
        var files = new List<DbcFile>();
        var dialog = new OpenFileDialog
        {
            Filter = "DBC files (*.dbc)|*.dbc",
            Multiselect = true
        };

        if (dialog.ShowDialog() == true)
        {
            var parser = new DbcFileParser();
            foreach (var path in dialog.FileNames)
            {
                var dbc = parser.Parse(path);
                files.Add(dbc);
            }
        }

        return files;
    }
}
