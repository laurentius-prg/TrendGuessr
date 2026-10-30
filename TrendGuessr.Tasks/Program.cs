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
            // List<string> keywords = await Tasks.GenerateKeywords(); Uncomment once done
            // TEMP
            List<string> keywords = new List<string>();
            keywords.Add("Minecraft");
            keywords.Add("MySpace");
            keywords.Add("Fortnite");
            //

            const string path = "../../../TrendGuessr/wwwroot/data/trends.json";
            Trend trend = new Trend() { Name = "A", PeakYear = 2020 };
            Trend trend1 = new Trend() { Name = "B", PeakYear = 2020 };
            Trend trend3 = new Trend() { Name = "C", PeakYear = 2021 };
            List<Trend> trends = new List<Trend>() { trend, trend1, trend3 };
            Output.SerializeTrendsToJSON(path, trends);
        }
    }
}
