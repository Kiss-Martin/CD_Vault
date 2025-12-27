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
    private bool _isPlaceholderVisible = true;
    private bool _isResultsVisible;
    private bool _isAnimating;
    private HashSet<string> _collectionKeys = new(StringComparer.OrdinalIgnoreCase);

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
            UpdatePlaceholderState();
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

    public bool IsPlaceholderVisible
    {
        get => _isPlaceholderVisible;
        private set
        {
            if (_isPlaceholderVisible == value)
            {
                return;
            }

            _isPlaceholderVisible = value;
            OnPropertyChanged();
        }
    }

    public bool IsResultsVisible
    {
        get => _isResultsVisible;
        private set
        {
            if (_isResultsVisible == value)
            {
                return;
            }

            _isResultsVisible = value;
            OnPropertyChanged();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCollectionKeysAsync();
        UpdatePlaceholderState();
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
                var key = BuildKey(result.Title, result.Artist);
                result.IsInCollection = _collectionKeys.Contains(key);
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

        if (album.IsInCollection)
        {
            var existing = await _database.FindByTitleArtistAsync(album.Title, album.Artist);
            if (existing is not null)
            {
                await _database.DeleteAsync(existing);
            }

            album.IsInCollection = false;
            _collectionKeys.Remove(BuildKey(album.Title, album.Artist));
            StatusMessage = $"\"{album.Title}\" eltávolítva a gyűjteményből.";
        }
        else
        {
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
            album.IsInCollection = true;
            _collectionKeys.Add(BuildKey(album.Title, album.Artist));
            StatusMessage = $"\"{album.Title}\" hozzáadva a gyűjteményhez.";
        }

        OnPropertyChanged(nameof(Results));
    }

    private async Task LoadCollectionKeysAsync()
    {
        var items = await _database.GetAllAsync();
        _collectionKeys = items
            .Select(item => BuildKey(item.Title, item.Artist))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private void UpdatePlaceholderState()
    {
        var isPlaceholderVisible = string.IsNullOrWhiteSpace(Query);
        IsPlaceholderVisible = isPlaceholderVisible;
        IsResultsVisible = !isPlaceholderVisible;

        if (IsPlaceholderVisible)
        {
            StartPlaceholderAnimation();
        }
        else
        {
            this.AbortAnimation("DiscSpin");
            _isAnimating = false;
        }
    }

    private void StartPlaceholderAnimation()
    {
        if (_isAnimating)
        {
            return;
        }

        _isAnimating = true;
        var animation = new Animation(value => PlaceholderDisc.Rotation = value, 0, 360);
        animation.Commit(this, "DiscSpin", 16, 2000, Easing.Linear, (_, _) =>
        {
            PlaceholderDisc.Rotation = 0;
            _isAnimating = false;
        }, () => IsPlaceholderVisible);
    }

    private static string BuildKey(string title, string artist)
    {
        return $"{title.Trim()}|{artist.Trim()}";
    }
}
