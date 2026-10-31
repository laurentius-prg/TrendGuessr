using System.Net.Http.Json;

namespace TrendGuessr
{
    public class GameDataService
    {
        private const string DATA_FILE = "data/trends.json";
        private readonly HttpClient HTTP;
        private List<Trend>? Cache;

        public GameDataService(HttpClient http) => HTTP = http;

        public async Task<Trend> GetRandomKeywordAsync()
        {
            Cache ??= await HTTP.GetFromJsonAsync<List<Trend>>(DATA_FILE);
            return Cache![Random.Shared.Next(Cache.Count)];
        }
    }
}
