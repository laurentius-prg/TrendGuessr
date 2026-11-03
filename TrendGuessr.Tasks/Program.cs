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
            List<Trend> trends = new List<Trend> {
            // 2005-2009
    new Trend("Crazy Frog", 2005),
    new Trend("Xbox 360", 2006),
    new Trend("PlayStation 3", 2006),
    new Trend("Heelys", 2006),
    new Trend("Motorola Razr", 2006),
    new Trend("MySpace", 2007),
    new Trend("Windows Vista", 2007),
    new Trend("Second Life", 2007),
    new Trend("Rickroll", 2008),
    new Trend("Guitar Hero", 2008),
    new Trend("Rock Band", 2008),
    new Trend("Spore", 2008),
    new Trend("Left 4 Dead", 2008),
    new Trend("Hannah Montana", 2008),
    new Trend("Twilight", 2009),
    new Trend("Snuggie", 2009),
    new Trend("Susan Boyle", 2009),
    new Trend("Swine Flu", 2009),
    new Trend("Bing", 2009),
    new Trend("Club Penguin", 2009),

    // 2010-2013
    new Trend("FarmVille", 2010),
    new Trend("Kinect", 2010),
    new Trend("Angry Birds", 2011),
    new Trend("Skyrim", 2011),
    new Trend("Nyan Cat", 2011),
    new Trend("Planking", 2011),
    new Trend("Rebecca Black", 2011),
    new Trend("Charlie Sheen", 2011),
    new Trend("Google Plus", 2011),
    new Trend("Occupy Wall Street", 2011),
    new Trend("Gangnam Style", 2012),
    new Trend("Draw Something", 2012),
    new Trend("Kony 2012", 2012),
    new Trend("SOPA", 2012),
    new Trend("Windows 8", 2012),
    new Trend("Harlem Shake", 2013),
    new Trend("Grumpy Cat", 2013),
    new Trend("Twerking", 2013),
    new Trend("Google Glass", 2013),
    new Trend("Candy Crush", 2013),
    new Trend("Breaking Bad", 2013),
    new Trend("Duck Dynasty", 2013),
    new Trend("Ouya", 2013),

    // 2014-2016
    new Trend("Flappy Bird", 2014),
    new Trend("Ice Bucket Challenge", 2014),
    new Trend("Doge", 2014),
    new Trend("Vine", 2014),
    new Trend("Ebola", 2014),
    new Trend("Ello", 2014),
    new Trend("Frozen", 2014),
    new Trend("Undertale", 2015),
    new Trend("Periscope", 2015),
    new Trend("Hoverboard", 2015),
    new Trend("The Dress", 2015),
    new Trend("Fifty Shades of Grey", 2015),
    new Trend("Clash of Clans", 2015),
    new Trend("Pokemon Go", 2016),
    new Trend("Overwatch", 2016),
    new Trend("Harambe", 2016),
    new Trend("Mannequin Challenge", 2016),
    new Trend("Damn Daniel", 2016),

    // 2017-2019
    new Trend("Fidget Spinner", 2017),
    new Trend("Bitcoin", 2017),
    new Trend("Fortnite", 2018),
    new Trend("PUBG", 2018),
    new Trend("Tide Pod Challenge", 2018),
    new Trend("Yanny or Laurel", 2018),
    new Trend("Juul", 2018),
    new Trend("Baby Shark", 2019),
    new Trend("Momo Challenge", 2019),
    new Trend("FaceApp", 2019),
    new Trend("Area 51 Raid", 2019),
    new Trend("Apex Legends", 2019),
    new Trend("Baby Yoda", 2019),
    new Trend("Avengers Endgame", 2019),

    // 2020-2022
    new Trend("Among Us", 2020),
    new Trend("Fall Guys", 2020),
    new Trend("Tiger King", 2020),
    new Trend("Animal Crossing", 2020),
    new Trend("Dalgona Coffee", 2020),
    new Trend("Murder Hornets", 2020),
    new Trend("Quibi", 2020),
    new Trend("Cyberpunk 2077", 2020),
    new Trend("Squid Game", 2021),
    new Trend("GameStop", 2021),
    new Trend("Dogecoin", 2021),
    new Trend("Clubhouse", 2021),
    new Trend("Wordle", 2022),
    new Trend("NFT", 2022),
    new Trend("Bored Ape", 2022),
    new Trend("BeReal", 2022),
    new Trend("Elden Ring", 2022),
    new Trend("Mastodon", 2022),

    // 2023-2025
    new Trend("ChatGPT", 2023),
    new Trend("Threads", 2023),
    new Trend("Barbie", 2023),
    new Trend("Oppenheimer", 2023),
    new Trend("Cybertruck", 2023),
    new Trend("Grimace Shake", 2023),
    new Trend("Baldur's Gate 3", 2023),
    new Trend("Eras Tour", 2023),
    new Trend("Apple Vision Pro", 2024),
    new Trend("Palworld", 2024),
    new Trend("Hawk Tuah", 2024),
    new Trend("Brat Summer", 2024),
    new Trend("Bluesky", 2024),
    new Trend("DeepSeek", 2025),
    new Trend("Labubu", 2025),
    new Trend("KPop Demon Hunters", 2025)
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
