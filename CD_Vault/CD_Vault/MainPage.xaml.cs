using CD_Vault.Pages;
using CD_Vault.Services;

namespace CD_Vault;

public partial class MainPage : ContentPage
{
    private readonly CdDatabase _database;
    private int _collectionCount;

    public MainPage(CdDatabase database)
    {
        InitializeComponent();
        _database = database;
        BindingContext = this;
    }

    public int CollectionCount
    {
        get => _collectionCount;
        set
        {
            if (_collectionCount == value)
            {
                return;
            }

            _collectionCount = value;
            OnPropertyChanged();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        CollectionCount = await _database.GetCountAsync();
    }

    private async void OnCollectionClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CollectionPage));
    }

    private async void OnDiscoverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(DiscoverPage));
    }
}
