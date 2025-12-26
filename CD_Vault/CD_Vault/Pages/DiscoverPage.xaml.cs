using System.Collections.ObjectModel;
using CD_Vault.Models;
using CD_Vault.Services;

namespace CD_Vault.Pages;

public partial class DiscoverPage : ContentPage
{
    private readonly AlbumSearchService _searchService;
    private readonly CdDatabase _database;
    private string _query = string.Empty;
    private bool _isSearching;
    private string _statusMessage = string.Empty;

    public DiscoverPage(AlbumSearchService searchService, CdDatabase database)
    {
        InitializeComponent();
        _searchService = searchService;
        _database = database;
        BindingContext = this;
    }

    public ObservableCollection<AlbumSearchResult> Results { get; } = new();

    public string Query
    {
        get => _query;
        set
        {
            if (_query == value)
            {
                return;
            }

            _query = value;
            OnPropertyChanged();
        }
    }

    public bool IsSearching
    {
        get => _isSearching;
        set
        {
            if (_isSearching == value)
            {
                return;
            }

            _isSearching = value;
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage == value)
            {
                return;
            }

            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Query))
        {
            StatusMessage = "Adj meg keresési kifejezést.";
            return;
        }

        IsSearching = true;
        StatusMessage = "Keresés folyamatban...";

        try
        {
            var results = await _searchService.SearchAlbumsAsync(Query.Trim());
            Results.Clear();
            foreach (var result in results)
            {
                Results.Add(result);
            }

            StatusMessage = results.Count == 0
                ? "Nem található album. Próbálj másik előadót vagy címet!"
                : $"{results.Count} találat érkezett.";
        }
        catch (Exception)
        {
            StatusMessage = "Hiba történt a keresés során. Ellenőrizd a kapcsolatot.";
        }
        finally
        {
            IsSearching = false;
        }
    }

    private async void OnAddFromSearchClicked(object sender, EventArgs e)
    {
        if (sender is not Button { CommandParameter: AlbumSearchResult album })
        {
            return;
        }

        var item = new CdItem
        {
            Title = album.Title,
            Artist = album.Artist,
            Genre = album.Genre,
            Year = album.ReleaseDate.Year,
            ArtworkUrl = album.ArtworkUrl,
            AddedAt = DateTime.UtcNow,
            Notes = "Hozzáadva a Felfedezés oldalról."
        };

        await _database.AddAsync(item);
        StatusMessage = $"\"{album.Title}\" hozzáadva a gyűjteményhez.";
    }
}
