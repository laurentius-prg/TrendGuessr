using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading;
using TrendGuessr.Services;

namespace TrendGuessr
{
    public class GameDataService
    {
        private const string DATA_FILE = "data/trends.json";
        private readonly HttpClient HTTP;
        private List<Trend>? Cache = null;
        private Queue<Trend> TrendQueue = new Queue<Trend>();

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
            if (TrendQueue.Count == 0)
                FillQueue();
            return TrendQueue.Dequeue();
        }
        private void ShuffleTrends()
        {
            if (Cache is null)
                throw new InvalidOperationException("Cache is not initialized.");
            int n = Cache.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                Trend value = Cache[k];
                Cache[k] = Cache[n];
                Cache[n] = value;
            }
        }
        private void FillQueue()
        {
            if (Cache is null)
                throw new InvalidOperationException("Cache is not initialized.");
            ShuffleTrends();
            foreach (var trend in Cache)
            {
                TrendQueue.Enqueue(trend);
            }
        }
    }
}
