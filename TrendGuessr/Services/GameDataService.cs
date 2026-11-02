using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace TrendGuessr
{
    public class GameDataService
    {
        private const string DATA_FILE = "data/trends.json";
        private readonly HttpClient HTTP;
        private List<Trend>? Cache = null;

        public GameDataService(HttpClient http) => HTTP = http;

        public async Task<Trend> GetRandomKeywordAsync()
        {
            if (Cache is null)
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, DATA_FILE);
                request.SetBrowserRequestCache(BrowserRequestCache.NoCache);

                using (var response = await HTTP.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    Cache = await response.Content.ReadFromJsonAsync<List<Trend>>();
                }
            }
            return Cache![Random.Shared.Next(Cache.Count)];
        }
    }
}
