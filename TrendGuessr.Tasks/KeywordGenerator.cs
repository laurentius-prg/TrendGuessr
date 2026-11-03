using DotNetEnv;
using GenerativeAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using static System.Net.WebRequestMethods;

namespace TrendGuessr.Tasks
{
    public class KeywordGenerator
    {
        private static readonly HttpClient Http = new HttpClient();

        private static readonly string[] geminiModels = {
            "models/gemini-3.5-flash", "models/gemini-3.8-flash", "models/gemini-2.5-pro" };
        private readonly string geminiApiKey;
        private const int MAX_ATTEMPTS = 4;
        private const int GENERATED_KEYWORD_COUNT = 10;

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
        public async Task<List<string>> GenerateKeywordsAsync()
        {
            // AI generated request.
            string request = "You are generating keywords for a trend-guessing game covering 2005 to today ({CURRENT_YEAR}).\r\n\r\nGenerate exactly " + GENERATED_KEYWORD_COUNT + " unique keywords.\r\n\r\nThe keywords will later be analyzed using Google Trends to determine when each keyword was most popular. DO NOT provide years, dates, popularity scores, or explanations.\r\n\r\nYour job is ONLY to generate good candidate keywords.\r\n\r\nTEMPORAL BALANCE IS EXTREMELY IMPORTANT.\r\n\r\nCreate a diverse mixture of keywords whose cultural relevance spans the entire 2005–{CURRENT_YEAR} period. Do not focus primarily on recent trends.\r\n\r\nAim for approximately equal representation of:\r\n- mid/late 2000s culture (2005–2009)\r\n- early 2010s culture (2010–2013)\r\n- mid 2010s culture (2014–2016)\r\n- late 2010s culture (2017–2019)\r\n- early 2020s culture (2020–2022)\r\n- recent culture (2023–today)\r\n\r\nInclude a broad mixture of:\r\n- websites and social networks\r\n- apps\r\n- technology and gadgets\r\n- video games\r\n- movies and TV shows\r\n- music and artists\r\n- internet memes\r\n- internet slang\r\n- celebrities\r\n- products and brands\r\n- major online phenomena\r\n- cultural trends\r\n- viral challenges\r\n- gaming trends\r\n\r\nPrefer keywords that:\r\n1. Had a distinct period of popularity.\r\n2. Are recognizable to a general internet user.\r\n3. Have enough Google search volume to produce meaningful Google Trends data.\r\n4. Are specific enough that Google Trends will not confuse them with many unrelated meanings.\r\n5. Can reasonably be guessed as belonging to a particular era.\r\n\r\nAvoid:\r\n- generic words such as \"internet\", \"technology\", \"music\", \"gaming\", or \"social media\"\r\n- extremely obscure references\r\n- keywords that have been consistently popular across the entire 2005–today period\r\n- duplicate or near-duplicate concepts\r\n- current events that are unlikely to remain meaningful\r\n- years or dates inside the keywords\r\n- explanations or descriptions\r\n- categories\r\n\r\nIMPORTANT:\r\nDo not simply generate a list of things that were popular in each era. Think of the final game: a player should see a keyword and have a reasonable chance of guessing approximately when it peaked.\r\n\r\nReturn ONLY a valid JSON array of strings.\r\n\r\nExample:\r\n[\"MySpace\", \"iPod\", \"Wii\", \"FarmVille\", \"Tumblr\", \"Minecraft\", \"Flappy Bird\", \"TikTok\"]";
            List<string> keywords = new List<string>();
            var googleAI = new GoogleAi(geminiApiKey);

            /* Lists google models available for use. Uncomment to see the list of models.
            
            var list = await googleAI.ListModelsAsync();

            foreach (var m in list.Models)
            {
                Console.WriteLine(m.Name);
            }
             */
            /* TEMPORARY TEST */ Console.WriteLine("Initiating AI");
            foreach (string geminiModel in geminiModels)
            {
                var model = googleAI.CreateGenerativeModel(geminiModel);
                /* TEMPORARY TEST */ Console.WriteLine("Model initiated.");
                for (int i = 1; i <= MAX_ATTEMPTS; i++)
                {
                    /* TEMPORARY TEST */Console.WriteLine("Attempt " + i + ":");
                    try
                    {
                        var response = await model.GenerateContentAsync(request);
                        /* TEMPORARY TEST */ Console.WriteLine("Response received.");
                        string text = response.Text();
                        /* TEMPORARY TEST */ Console.WriteLine("Converted to text.");
                        Console.WriteLine(text);
                        // Outputs list of keywords to console for verification.
                        Console.WriteLine();
                        keywords = JsonSerializer.Deserialize<List<string>>(text);
                        /* TEMPORARY TEST */ Console.WriteLine("Converted to keywords.");
                        foreach (var keyword in keywords)
                        {
                            Console.WriteLine(keyword);
                        }
                        Console.WriteLine(keywords.Count);
                        return keywords;
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
                /* TEMPORARY TEST */ Console.WriteLine("Fetching trend " + keyword + "...");
                var trend = await FetchTrendAsync(keyword);
                /* TEMPORARY TEST */ Console.WriteLine("Trend fetched: " + trend.ToString());
                if (trend != null) trends.Add(trend);   // skip bad keywords, keep the rest
            }

            return trends;
        }
        private async Task<Trend> FetchTrendAsync(string keyword)
        {
            string url = "https://serpapi.com/search.json?engine=google_trends&q="
                + Uri.EscapeDataString(keyword) + 
                "&data_type=TIMESERIES&date=all&api_key=" + serpApiKey;

            string body = await Http.GetStringAsync(url);

            using (JsonDocument doc = JsonDocument.Parse(body))
            {
                JsonElement interest, timeline;
                if (!doc.RootElement.TryGetProperty("interest_over_time", out interest) ||
                    !interest.TryGetProperty("timeline_date", out timeline) ||
                    timeline.GetArrayLength() == 0)
                    return null;

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

                return new Trend(keyword, peakYear);
            }
        }
        public async Task<string> FetchImageURL(string keyword)
        {
            string url = "https://commons.wikimedia.org/w/api.php?action=query" +
                "&generator=search" +
                $"&gsrsearch={Uri.EscapeDataString(keyword)}" +
                "&gsrnamespace=6&gsrlimit=10&gsrwhat=text&prop=imageinfo" +
                "&iiprop=url&iiurlwidth=1200&format=json&formatversion=2";

            using (JsonDocument doc = JsonDocument.Parse(await Http.GetStringAsync(url)))
            {
                if (!doc.RootElement.TryGetProperty("query", out JsonElement query))
                    return null;
                if (!query.TryGetProperty("pages", out JsonElement pages))
                    return null;

                foreach (JsonElement page in query.GetProperty("pages").EnumerateArray()
                             .OrderBy(p => p.TryGetProperty("index", out var i) ? i.GetInt32() : int.MaxValue))
                {
                    Console.WriteLine("Checking page: " + page.GetProperty("title").GetString());
                    if (!page.TryGetProperty("imageinfo", out JsonElement imgInfo))
                        continue;
                    Console.WriteLine("Image info found, length: " + imgInfo.GetArrayLength());
                    if (imgInfo.GetArrayLength() == 0)
                        continue;
                    Console.WriteLine("Image info first element: " + imgInfo[0].ToString());
                    if (imgInfo[0].TryGetProperty("thumburl", out JsonElement thumbUrl))
                        return thumbUrl.GetString();
                    Console.WriteLine("Thumb URL not found, checking for original URL...");
                    if (imgInfo[0].TryGetProperty("url", out JsonElement originalUrl))
                        return originalUrl.GetString();
                }

                return null;
            }
        }
    }
}
