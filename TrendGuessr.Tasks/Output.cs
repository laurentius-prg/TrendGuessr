using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendGuessr.Tasks
{
    public static class Output
    {
        public static void SerializeTrendsToJSON(string path, List<Trend> trends)
        {
            var json = new JavaScriptSerializer().Serialize(trends);
            File.WriteAllText(path, json);
        }
    }
}
