using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendGuessr.Tasks
{
    public class Trend
    {
        public string Keyword { get; set; }
        public int PeakYear { get; set; }
        public string ImageURL { get; set; }
        public Trend(string keyword, int peakYear)
        {
            Keyword = keyword;
            PeakYear = peakYear;
            ImageURL = null;
        }
        public override string ToString()
        {
            return string.Format(Keyword + " " + PeakYear);
        }
    }
}
