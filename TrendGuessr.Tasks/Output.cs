using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace TrendGuessr.Tasks
{
    public static class Output
    {
        public static void SerializeTrendsToJSON(string path, List<Trend> trends)
        {
            if (File.Exists(path))
            {
                string json = JsonSerializer.Serialize(trends);
                File.WriteAllText(path, json);
            }
            else
                throw new FileNotFoundException($"The file at path '{path}' does not exist.");
        }
        public static Dictionary<string, Trend> DeserializeTrendsFromJSON(string path)
        {
            if (File.Exists(path))
            {
                /*
                try
                {
                */
                    List<Trend> trends = JsonSerializer.Deserialize<List<Trend>>(File.ReadAllText(path));
                return trends.ToDictionary(t => t.Keyword, t => t);
                /*
                }
                catch
                {
                    return new Dictionary<string, Trend>();
                }
                */
            }
            else
                throw new FileNotFoundException($"The file at path '{path}' does not exist.");
        }


    }
}
