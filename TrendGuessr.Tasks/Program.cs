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
            const int GENERATED_KEYWORD_COUNT = 20;
            const string path = "../../../TrendGuessr/wwwroot/data/trends.json";
            Dictionary<string, Trend> existingTrends = Output.DeserializeTrendsFromJSON(path);
            KeywordGenerator generator = new KeywordGenerator(4);
            string keywordRequest = "You are generating keywords for a trend-guessing game covering 2005 to today ({CURRENT_YEAR}).\r\n\r\nGenerate exactly " + GENERATED_KEYWORD_COUNT + " unique keywords.\r\n\r\nThe keywords will later be analyzed using Google Trends to determine when each keyword was most popular. DO NOT provide years, dates, popularity scores, or explanations.\r\n\r\nYour job is ONLY to generate good candidate keywords.\r\n\r\nTEMPORAL BALANCE IS EXTREMELY IMPORTANT.\r\n\r\nCreate a diverse mixture of keywords whose cultural relevance spans the entire 2005–{CURRENT_YEAR} period. Do not focus primarily on recent trends.\r\n\r\nAim for approximately equal representation of:\r\n- mid/late 2000s culture (2005–2009)\r\n- early 2010s culture (2010–2013)\r\n- mid 2010s culture (2014–2016)\r\n- late 2010s culture (2017–2019)\r\n- early 2020s culture (2020–2022)\r\n- recent culture (2023–today)\r\n\r\nInclude a broad mixture of:\r\n- websites and social networks\r\n- apps\r\n- technology and gadgets\r\n- video games\r\n- movies and TV shows\r\n- music and artists\r\n- internet memes\r\n- internet slang\r\n- celebrities\r\n- products and brands\r\n- major online phenomena\r\n- cultural trends\r\n- viral challenges\r\n- gaming trends\r\n\r\nPrefer keywords that:\r\n1. Had a distinct period of popularity.\r\n2. Are recognizable to a general internet user.\r\n3. Have enough Google search volume to produce meaningful Google Trends data.\r\n4. Are specific enough that Google Trends will not confuse them with many unrelated meanings.\r\n5. Can reasonably be guessed as belonging to a particular era.\r\n\r\nAvoid:\r\n- generic words such as \"internet\", \"technology\", \"music\", \"gaming\", or \"social media\"\r\n- extremely obscure references\r\n- keywords that have been consistently popular across the entire 2005–today period\r\n- duplicate or near-duplicate concepts\r\n- current events that are unlikely to remain meaningful\r\n- years or dates inside the keywords\r\n- explanations or descriptions\r\n- categories\r\n\r\nIMPORTANT:\r\nDo not simply generate a list of things that were popular in each era. Think of the final game: a player should see a keyword and have a reasonable chance of guessing approximately when it peaked.\r\n\r\nReturn ONLY a valid JSON array of strings.\r\n\r\nExample:\r\n[\"MySpace\", \"iPod\", \"Wii\", \"FarmVille\", \"Tumblr\", \"Minecraft\", \"Flappy Bird\", \"TikTok\"]";
            List<string> keywords = await generator.GenerateGeminiResponseAsync(keywordRequest);
            List<Trend> trends = await generator.GenerateTrendListAsync(keywords);
            foreach (Trend trend in trends)
                existingTrends[trend.Keyword] = trend;
            Output.SerializeTrendsToJSON(path, existingTrends.Values.ToList());
        }
    }
}
