using System.Text.Json.Serialization;

namespace CD_Vault.Models;

public class AlbumSearchResult
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
}
