using System.Net.Http.Json;

namespace TrendGuessr
{
    public class GameDataService
    {
        const string DATA_FILE = "data/trends.json";
        private readonly HttpClient _http;
        private List<Trend>? _cache;

        public GameDataService(HttpClient http) => _http = http;

        public async Task<Trend> GetRandomKeywordAsync()
        {
            _cache ??= await _http.GetFromJsonAsync<List<Trend>>(DATA_FILE);
            return _cache![Random.Shared.Next(_cache.Count)];
        }
    }
}
