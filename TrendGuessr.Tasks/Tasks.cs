using System;
using System.Collections.Generic;
using DotNetEnv;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using GenerativeAI;
//using Newtonsoft.Json.Linq;

namespace TrendGuessr.Tasks
{
    public static class Tasks
    {
        public static async Task<List<string>> GenerateKeywords()
        {
            Env.Load();
            // AI generated request.
            string request = "You are generating keywords for a trend-guessing game covering 2005 to today ({CURRENT_YEAR}).\r\n\r\nGenerate exactly 500 unique keywords.\r\n\r\nThe keywords will later be analyzed using Google Trends to determine when each keyword was most popular. DO NOT provide years, dates, popularity scores, or explanations.\r\n\r\nYour job is ONLY to generate good candidate keywords.\r\n\r\nTEMPORAL BALANCE IS EXTREMELY IMPORTANT.\r\n\r\nCreate a diverse mixture of keywords whose cultural relevance spans the entire 2005–{CURRENT_YEAR} period. Do not focus primarily on recent trends.\r\n\r\nAim for approximately equal representation of:\r\n- mid/late 2000s culture (2005–2009)\r\n- early 2010s culture (2010–2013)\r\n- mid 2010s culture (2014–2016)\r\n- late 2010s culture (2017–2019)\r\n- early 2020s culture (2020–2022)\r\n- recent culture (2023–today)\r\n\r\nInclude a broad mixture of:\r\n- websites and social networks\r\n- apps\r\n- technology and gadgets\r\n- video games\r\n- movies and TV shows\r\n- music and artists\r\n- internet memes\r\n- internet slang\r\n- celebrities\r\n- products and brands\r\n- major online phenomena\r\n- cultural trends\r\n- viral challenges\r\n- gaming trends\r\n\r\nPrefer keywords that:\r\n1. Had a distinct period of popularity.\r\n2. Are recognizable to a general internet user.\r\n3. Have enough Google search volume to produce meaningful Google Trends data.\r\n4. Are specific enough that Google Trends will not confuse them with many unrelated meanings.\r\n5. Can reasonably be guessed as belonging to a particular era.\r\n\r\nAvoid:\r\n- generic words such as \"internet\", \"technology\", \"music\", \"gaming\", or \"social media\"\r\n- extremely obscure references\r\n- keywords that have been consistently popular across the entire 2005–today period\r\n- duplicate or near-duplicate concepts\r\n- current events that are unlikely to remain meaningful\r\n- years or dates inside the keywords\r\n- explanations or descriptions\r\n- categories\r\n\r\nIMPORTANT:\r\nDo not simply generate a list of things that were popular in each era. Think of the final game: a player should see a keyword and have a reasonable chance of guessing approximately when it peaked.\r\n\r\nReturn ONLY a valid JSON array of strings.\r\n\r\nExample:\r\n[\"MySpace\", \"iPod\", \"Wii\", \"FarmVille\", \"Tumblr\", \"Minecraft\", \"Flappy Bird\", \"TikTok\"]";
            var apiKey = System.Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidDataException("No Gemini secret.");
            List<string> keywords = new List<string>();

            var googleAI = new GoogleAi(apiKey);

            /*
            ListModelsResponse list = await googleAI.ListModelsAsync();

            foreach (var m in list.Models)
            {
                Console.WriteLine(m.Name);
            }
            */
            var model = googleAI.CreateGenerativeModel("models/gemini-pro-latest");
            try
            {
                var response = await model.GenerateContentAsync(request);
                string text = response.Text();
                Console.WriteLine(text);
                keywords = System.Text.Json.JsonSerializer.Deserialize<List<string>>(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed: {ex.GetType().Name}: {ex.Message}");
            }
            Console.WriteLine();
            foreach (var keyword in keywords)
            {
                Console.WriteLine(keyword);
            }
            Console.WriteLine(keywords.Count);
            return keywords;
        }
        public static async Task<List<Trend>> GenerateTrendList(List<string> keywords)
        {
            Env.Load();
            var apiKey = System.Environment.GetEnvironmentVariable("SERPAPI_KEY");
            List<Trend> trends = new List<Trend>();
            var httpClient = new HttpClient();
            foreach (string keyword in keywords)
            {
                string url = "https://serpapi.com/search.json"
                    + "?engine=google_trends"
                    + $"&q={Uri.EscapeDataString(keyword)}"
                    + "&data_type=TIMESERIES&date=all"
                    + $"&api_key={apiKey}";
                var json = await httpClient.GetAsync(url);
                var timeline = json["interest_over_time"]?["timeline_date"] as JArray;
                if (timeline == null || timeline.Count == 0)
                    return null;

                var yearPopularitySums = new Dictionary<int, (double Sum, int Count)> ();
                foreach (var point in timeline)
                {
                    long seconds = long.Parse((string)point["timestamp"]);
                    int year = DateTimeOffset.FromUnixTimeSeconds(seconds).Year;

                    // skip 2004 and the partial current year
                    if (year < 2005 || year >= DateTime.UtcNow.Year) continue;

                    int value = (int)point["values"][0]["extracted_value"];

                    yearPopularitySums.TryGetValue(year, out var t);
                    yearPopularitySums[year] = (t.Sum + value, t.Count + 1);
                }

            }
            return trends;
        }

    }
}
