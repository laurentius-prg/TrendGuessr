using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrendGuessr.Tasks
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string path = "../../../TrendGuessr/wwwroot/data/trends.json";
            Dictionary<string, Trend> existingTrends = Output.DeserializeTrendsFromJSON(path);
            KeywordGenerator generator = new KeywordGenerator();
            /*
            List<string> keywords = await generator.GenerateKeywordsAsync();
            List<Trend> trends = await generator.GenerateTrendListAsync(keywords);
            */
            List<Trend> trends = new List<Trend>
            {
                new Trend("Minecraft", 2011),
                new Trend("Terraria", 2018),
                new Trend("AI", 2023),
                new Trend("League of Legends", 2011),
                new Trend("Roblox", 2020),
                new Trend("Microsoft Teams", 2020),
                new Trend("LOL", 2018)
            };
            foreach (Trend trend in trends)
            {
                if (existingTrends.ContainsKey(trend.Keyword))
                {
                    existingTrends[trend.Keyword] = trend;
                }
                else
                {
                    trend.ImageURL = await generator.FetchImageURL(trend.Keyword);
                    existingTrends.Add(trend.Keyword, trend);
                }
            }
            Output.SerializeTrendsToJSON(path, existingTrends.Values.ToList());
        }
    }
}
