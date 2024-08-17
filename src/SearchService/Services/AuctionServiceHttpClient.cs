using System;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionServiceHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AuctionServiceHttpClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<List<Item>> GetItemsForSearchDB()
    {
        // get the latest updatedAt date in items (searchDB)
        var lastUpdated = await DB.Find<Item, string>()
            .Sort(m => m.Descending(a => a.UpdatedAt))
            .Project(m => m.UpdatedAt.ToString())
            .ExecuteFirstAsync();

        // make a request to the auction service to bring auctions after the last updatedAt date (for seeding) (auctionsDB)
        return await _httpClient.GetFromJsonAsync<List<Item>>($"{_config["AuctionServiceUrl"]}/api/auctions?date={lastUpdated}");
    }
}
