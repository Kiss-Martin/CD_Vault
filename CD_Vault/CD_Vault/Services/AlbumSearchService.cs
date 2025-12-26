using System.Net.Http.Json;
using System.Text.Json;
using CD_Vault.Models;

namespace CD_Vault.Services;

public class AlbumSearchService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public AlbumSearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AlbumSearchResult>> SearchAlbumsAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<AlbumSearchResult>();
        }

        var url = $"https://itunes.apple.com/search?term={Uri.EscapeDataString(query)}&media=music&entity=album&limit=25";
        var response = await _httpClient.GetFromJsonAsync<SearchResponse>(url, _serializerOptions, cancellationToken);

        return response?.Results ?? new List<AlbumSearchResult>();
    }

    private sealed class SearchResponse
    {
        public int ResultCount { get; set; }

        public List<AlbumSearchResult> Results { get; set; } = new();
    }
}
