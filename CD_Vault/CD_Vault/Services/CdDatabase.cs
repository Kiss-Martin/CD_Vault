using CD_Vault.Models;
using Microsoft.Maui.Storage;
using SQLite;

namespace CD_Vault.Services;

public class CdDatabase
{
    private readonly SQLiteAsyncConnection _connection;
    private bool _initialized;

    public CdDatabase()
    {
        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "cdvault.db3");
        _connection = new SQLiteAsyncConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        await _connection.CreateTableAsync<CdItem>();
        _initialized = true;
    }

    public async Task<List<CdItem>> GetAllAsync()
    {
        await InitializeAsync();
        return await _connection.Table<CdItem>()
            .OrderByDescending(item => item.AddedAt)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        await InitializeAsync();
        return await _connection.Table<CdItem>().CountAsync();
    }

    public async Task<CdItem?> FindByTitleArtistAsync(string title, string artist)
    {
        await InitializeAsync();
        return await _connection.Table<CdItem>()
            .Where(item => item.Title == title && item.Artist == artist)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(CdItem item)
    {
        await InitializeAsync();
        await _connection.InsertAsync(item);
    }

    public async Task DeleteAsync(CdItem item)
    {
        await InitializeAsync();
        await _connection.DeleteAsync(item);
    }
}
