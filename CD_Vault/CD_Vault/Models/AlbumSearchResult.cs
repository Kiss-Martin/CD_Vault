using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CD_Vault.Models;

public class AlbumSearchResult : INotifyPropertyChanged
{
    [JsonPropertyName("collectionName")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("artistName")]
    public string Artist { get; set; } = string.Empty;

    [JsonPropertyName("primaryGenreName")]
    public string Genre { get; set; } = string.Empty;

    [JsonPropertyName("releaseDate")]
    public DateTime ReleaseDate { get; set; }

    [JsonPropertyName("artworkUrl100")]
    public string ArtworkUrl { get; set; } = string.Empty;

    private bool _isInCollection;

    public bool IsInCollection
    {
        get => _isInCollection;
        set
        {
            if (_isInCollection == value)
            {
                return;
            }

            _isInCollection = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ActionLabel));
        }
    }

    public string ActionLabel => IsInCollection ? "Eltávolítás" : "Mentés";

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
