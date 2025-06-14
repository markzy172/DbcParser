using DbcParser.Gui.Commands;
using DbcParser.Gui.Services;
using DbcParser.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DbcParser.Gui.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private List<DbcMessage> _allMessages = new();

    public ObservableCollection<DbcMessage> FilteredMessages { get; } = new();

    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (_searchQuery != value)
            {
                _searchQuery = value;
                OnPropertyChanged();
                //FilterMessages();
            }
        }
    }

    public ICommand LoadFilesCommand { get; }
    public ICommand SearchCommand { get; }

    public MainViewModel()
    {
        LoadFilesCommand = new RelayCommand(_ => LoadDbcFiles());
        SearchCommand = new RelayCommand(_ => FilterMessages());
    }

    private void LoadDbcFiles()
    {
        _allMessages.Clear();
        FilteredMessages.Clear();

        var files = DbcFileLoader.LoadFiles();

        foreach (var file in files)
        {
            _allMessages.AddRange(file.Messages);
        }

        FilterMessages();
    }

    private void FilterMessages()
    {
        FilteredMessages.Clear();

        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            foreach (var msg in _allMessages)
                FilteredMessages.Add(msg);
        }
        else
        {
            string lowerQuery = SearchQuery.ToLowerInvariant();

            foreach (var msg in _allMessages)
            {
                bool messageMatches = msg.Name.ToLowerInvariant().Contains(lowerQuery);

                bool signalMatches = msg.Signals.Any(sig => sig.Name.ToLowerInvariant().Contains(lowerQuery));

                if (messageMatches || signalMatches)
                    FilteredMessages.Add(msg);
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

