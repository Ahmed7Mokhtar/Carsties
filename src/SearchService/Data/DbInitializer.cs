using System;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDBConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();
        
        #region Sync Comunication With Auction Service using Http

        using var scope = app.Services.CreateScope();

        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

        var items = await httpClient.GetItemsForSearchDB();
        
        Console.WriteLine(items.Count + " Returned From the Auction Serviced");

        if(items.Count > 0)
            await DB.SaveAsync(items);
        #endregion

        #region Seed Data
        // if(count == 0) 
        // {
        //     Console.WriteLine("No Date to Seed!");
        //     var itemData = await File.ReadAllTextAsync("Data/auctions.json");
        //     var options = new JsonSerializerOptions 
        //     {
        //         PropertyNameCaseInsensitive = true
        //     };

        //     var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);
            
        //     await DB.SaveAsync(items);
        // }
        #endregion
    }
}
