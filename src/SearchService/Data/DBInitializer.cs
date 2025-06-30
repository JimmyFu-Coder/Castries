
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data
{
    public class DBInitializer
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync("SearchDB", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDBConnection")));
            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();

            var count = await DB.CountAsync<Item>();
            if (count == 0)
            {
                var itemData = await File.ReadAllTextAsync("Data/auctions.json");
                var items = JsonSerializer.Deserialize<List<Item>>(itemData, _jsonOptions);
                await DB.SaveAsync(items);
            }
        }
    }
}