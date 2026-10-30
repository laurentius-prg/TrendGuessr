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
            KeywordGenerator generator = new KeywordGenerator();
            List<string> keywords = await generator.GenerateKeywordsAsync();
            List<Trend> trends = await generator.GenerateTrendListAsync(keywords);
            Output.SerializeTrendsToJSON(path, trends);
        }
    }
}
