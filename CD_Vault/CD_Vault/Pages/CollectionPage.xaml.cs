using System.Collections.ObjectModel;
using CD_Vault.Models;
using CD_Vault.Services;

namespace CD_Vault.Pages;

public partial class CollectionPage : ContentPage
{
    private readonly CdDatabase _database;
    private string _titleInput = string.Empty;
    private string _artistInput = string.Empty;
    private string _genreInput = string.Empty;
    private string _yearInput = string.Empty;
    private string _notesInput = string.Empty;
    private string _formMessage = string.Empty;

    public CollectionPage(CdDatabase database)
    {
        InitializeComponent();
        _database = database;
        BindingContext = this;
    }

    public ObservableCollection<CdItem> Items { get; } = new();

    public string TitleInput
    {
        get => _titleInput;
        set
        {
            if (_titleInput == value)
            {
                return;
            }

            _titleInput = value;
            OnPropertyChanged();
        }
    }

    public string ArtistInput
    {
        get => _artistInput;
        set
        {
            if (_artistInput == value)
            {
                return;
            }

            _artistInput = value;
            OnPropertyChanged();
        }
    }

    public string GenreInput
    {
        get => _genreInput;
        set
        {
            if (_genreInput == value)
            {
                return;
            }

            _genreInput = value;
            OnPropertyChanged();
        }
    }

    public string YearInput
    {
        get => _yearInput;
        set
        {
            if (_yearInput == value)
            {
                return;
            }

            _yearInput = value;
            OnPropertyChanged();
        }
    }

    public string NotesInput
    {
        get => _notesInput;
        set
        {
            if (_notesInput == value)
            {
                return;
            }

            _notesInput = value;
            OnPropertyChanged();
        }
    }

    public string FormMessage
    {
        get => _formMessage;
        set
        {
            if (_formMessage == value)
            {
                return;
            }

            _formMessage = value;
            OnPropertyChanged();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadItemsAsync();
    }

    private async Task LoadItemsAsync()
    {
        Items.Clear();
        var items = await _database.GetAllAsync();
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleInput) || string.IsNullOrWhiteSpace(ArtistInput))
        {
            FormMessage = "Adj meg címet és előadót is.";
            return;
        }

        int.TryParse(YearInput, out var yearValue);

        var item = new CdItem
        {
            Title = TitleInput.Trim(),
            Artist = ArtistInput.Trim(),
            Genre = GenreInput.Trim(),
            Year = yearValue,
            Notes = NotesInput.Trim(),
            AddedAt = DateTime.UtcNow
        };

        await _database.AddAsync(item);

        TitleInput = string.Empty;
        ArtistInput = string.Empty;
        GenreInput = string.Empty;
        YearInput = string.Empty;
        NotesInput = string.Empty;
        FormMessage = "Hozzáadva a gyűjteményhez!";

        await LoadItemsAsync();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: CdItem item })
        {
            await _database.DeleteAsync(item);
            Items.Remove(item);
        }
    }
}
