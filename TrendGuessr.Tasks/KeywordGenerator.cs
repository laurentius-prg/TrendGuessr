using DotNetEnv;
using GenerativeAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TrendGuessr.Tasks
{
    public class KeywordGenerator
    {
        private static readonly HttpClient Http = new HttpClient();

        private static readonly string[] geminiModels = {
            "models/gemini-3.5-flash", "models/gemini-3.8-flash", "models/gemini-2.5-pro" };
        private readonly string geminiApiKey;
        private readonly int MAX_ATTEMPTS = 4;

        private readonly string serpApiKey;
        public KeywordGenerator()
        {
            Env.TraversePath().Load();
            geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (string.IsNullOrEmpty(geminiApiKey))
                throw new InvalidOperationException("GEMINI_API_KEY environment variable is not set.");
            serpApiKey = Environment.GetEnvironmentVariable("SERPAPI_KEY");
            if (string.IsNullOrEmpty(serpApiKey))
                throw new InvalidOperationException("SERPAPI_KEY environment variable is not set.");
            Http.DefaultRequestHeaders.UserAgent.ParseAdd("TrendGuessr/1.0 (https://github.com/laurentius-prg/TrendGuessr)");
        }
        public KeywordGenerator(int maximumGeminiAttempts)
        {
            Env.TraversePath().Load();
            geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (string.IsNullOrEmpty(geminiApiKey))
                throw new InvalidOperationException("GEMINI_API_KEY environment variable is not set.");
            serpApiKey = Environment.GetEnvironmentVariable("SERPAPI_KEY");
            if (string.IsNullOrEmpty(serpApiKey))
                throw new InvalidOperationException("SERPAPI_KEY environment variable is not set.");
            Http.DefaultRequestHeaders.UserAgent.ParseAdd("TrendGuessr/1.0 (https://github.com/laurentius-prg/TrendGuessr)");
            MAX_ATTEMPTS = maximumGeminiAttempts;
        }
        public async Task<List<string>> GenerateGeminiResponseAsync(string request)
        {
            List<string> result = new List<string>();
            var googleAI = new GoogleAi(geminiApiKey);

            /* Lists google models available for use. Uncomment to see the list of models.
            
            var list = await googleAI.ListModelsAsync();

            foreach (var m in list.Models)
            {
                Console.WriteLine(m.Name);
            }
             */
            /* TEST OUTPUT */ Console.WriteLine("Initiating AI");
            foreach (string geminiModel in geminiModels)
            {
                var model = googleAI.CreateGenerativeModel(geminiModel);
                /* TEST OUTPUT */ Console.WriteLine("Model initiated.");
                for (int i = 1; i <= MAX_ATTEMPTS; i++)
                {
                    /* TEST OUTPUT */ Console.WriteLine("Attempt " + i + ":");
                    try
                    {
                        var response = await model.GenerateContentAsync(request);
                        /* TEST OUTPUT */  Console.WriteLine("Response received.");
                        string text = response.Text();
                        /* TEST OUTPUT */ Console.WriteLine("Converted to text.");
                        Console.WriteLine(text);
                        // Outputs list of keywords to console for verification.
                        Console.WriteLine();
                        result = JsonSerializer.Deserialize<List<string>>(text);
                        /* TEST OUTPUT */  Console.WriteLine("Converted to keywords.");
                        foreach (var keyword in result)
                        {
                            Console.WriteLine(keyword);
                        }
                        Console.WriteLine(result.Count);
                        return result;
                    }
                    catch (Exception ex) when (i < MAX_ATTEMPTS)
                    {
                        int delaySeconds = 5 * (int)Math.Pow(2, i - 1); // Exponential backoff: 5, 10, 20 seconds
                        Console.WriteLine($"failed: {ex.GetType().Name}: {ex.Message}");
                        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"failed: {ex.GetType().Name}: {ex.Message}");
                    }
                }
            }
            throw new InvalidOperationException("Models were unavailable");
        }
        public async Task<List<Trend>> GenerateTrendListAsync(List<string> keywords)
        {
            var trends = new List<Trend>();

            foreach (var keyword in keywords)
            {
                /* TEST OUTPUT */ Console.WriteLine("Fetching trend " + keyword + "...");
                Trend trend = await FetchTrendAsync(keyword);
                /* TEST OUTPUT */ Console.WriteLine("Trend fetched: " + trend.ToString());
                if (trend != null) trends.Add(trend);   // skip bad keywords, keep the rest
            }

            return trends;
        }
        private async Task<Trend> FetchTrendAsync(string keyword)
        {
            string peakYearUrl = "https://serpapi.com/search.json?engine=google_trends&q="
                + Uri.EscapeDataString(keyword) +
                "&data_type=TIMESERIES&date=all&api_key=" + serpApiKey;
            string peakYearBody = await Http.GetStringAsync(peakYearUrl);
            string imageUrl = "https://serpapi.com/search.json?engine=google_images&q="
            + Uri.EscapeDataString(keyword) + "&api_key=" + serpApiKey;
            string imageBody = await Http.GetStringAsync(imageUrl);
            int? peakYear = CalculatePeakYearFromJson(peakYearBody);
            string imageURL = GetImageUrlFromJson(imageBody);
            if (peakYear == null)
                throw new ArgumentNullException("peakYear", "Peak year could not be determined for keyword: " + keyword);
            return new Trend(keyword, (int)peakYear, imageURL);
        }
        private int? CalculatePeakYearFromJson(string body)
        {
            using (JsonDocument doc = JsonDocument.Parse(body))
            {
                if (doc.RootElement.TryGetProperty("error", out JsonElement error))
                {
                    /* TEST OUTPUT */
                    Console.WriteLine("root error: " + error.GetString());
                    return null;
                }

                JsonElement interest, timeline;
                if (!doc.RootElement.TryGetProperty("interest_over_time", out interest) ||
                    !interest.TryGetProperty("timeline_data", out timeline) ||
                    timeline.GetArrayLength() == 0)
                {
                    /* TEST OUTPUT */
                    Console.WriteLine("interest error:" + body.ToString());
                    return null;
                }

                int currentYear = DateTime.UtcNow.Year;

                // average popularity per year, skipping 2004 and the partial current year
                int peakYear = timeline.EnumerateArray()
                    .Select(p => new
                    {
                        DateTimeOffset.FromUnixTimeSeconds(long.Parse(p.GetProperty("timestamp").GetString())).Year,
                        Value = p.GetProperty("values")[0].GetProperty("extracted_value").GetInt32()
                    })
                    .Where(p => p.Year >= 2005 && p.Year < currentYear)
                    .GroupBy(p => p.Year)
                    .OrderByDescending(g => g.Average(p => p.Value))
                    .First()
                    .Key;
                /* TEST OUTPUT */
                Console.WriteLine("Year fetched: " + peakYear);
                return peakYear;
            }
        }
        private string GetImageUrlFromJson(string body)
        {
            using (JsonDocument doc = JsonDocument.Parse(body))
            {
                if (doc.RootElement.TryGetProperty("error", out JsonElement error))
                {
                    /* TEST OUTPUT */ Console.WriteLine("root error: " + error.GetString());
                    return null;
                }
                JsonElement images;
                if (!doc.RootElement.TryGetProperty("images_results", out images) ||
                    images.GetArrayLength() == 0)
                {
                    /* TEST OUTPUT */ Console.WriteLine("images error:" + body.ToString());
                    return null;
                }
                string imageUrl = images[0].GetProperty("original").GetString();
                /* TEST OUTPUT */ Console.WriteLine("Image URL fetched: " + imageUrl);
                return imageUrl;
            }
        }
    }
}
