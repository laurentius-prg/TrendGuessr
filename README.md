# TrendGuessr

A retro-styled guessing game: you see a keyword (a website, game, meme, gadget, show...) and guess the year it was most popular on Google between 2005 and today. The answers are computed from real Google Trends data, not hand-written.

Play it: https://trend-guessr-kappa.vercel.app/

## How it works

The solution has two projects:

1. **TrendGuessr.Tasks** (console app, run offline). It builds the keywords required for the game:
   1. Asks Gemini for a list of keywords spread across 2005-today.
   2. Fetches each keyword's Google Trends timeline and their images through [SerpApi](https://serpapi.com/)
   3. Averages each months' interest per year and picks the peak year, fetches image URL.
   4. Writes the result to `TrendGuessr/wwwroot/data/trends.json`.
2. **TrendGuessr** (Blazor WebAssembly app). The game itself. It only reads `trends.json`, so it needs no server, no database and no API keys at runtime.

```
TrendGuessr/             Blazor WASM front end (this is what gets deployed)
  Layout/                MainLayout
  Models/                Trend model
  Pages/                 Home, Play, Results
  Services/              Game state and data logic
  wwwroot/
    css/app.css          Global styles (NES.css theme)
    data/trends.json     Generated question bank
      vercel.json        Config for Vercel
  vercel.json            Same config, read when Vercel builds from Git
TrendGuessr.Tasks/       Console app that generates trends.json
  KeywordGenerator.cs    Keyword generator alongside said keyword data (peak year, image) gathering
  Output.cs              trends.json serialization and deserialization
  Program.cs             Pipeline entry point
```

## Getting started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Visual Studio 2022 or the `dotnet` CLI
- API keys, only if you want to generate data:
  - a [SerpApi](https://serpapi.com/) key
  - a [Gemini API](https://ai.google.dev/) key

### Run the game locally

The repo includes a generated `trends.json`, so you can play right away:

```bash
cd TrendGuessr
dotnet run
```

Then open the URL printed in the console.

### Generate trend data

1. In `TrendGuessr.Tasks`, create a `.env` file:

   ```
   SERPAPI_KEY=your_serpapi_key
   GEMINI_API_KEY=your_gemini_key
   ```

2. Run the pipeline:

   ```bash
   cd TrendGuessr.Tasks
   dotnet run
   ```

3. This updates existing trends in `TrendGuessr/wwwroot/data/trends.json` and adds non-existing ones with generated trends.

Note: each run uses SerpApi quota (two requests per keyword) and Gemini quota.

## Data format

`trends.json` is a list of objects like:

```json
[
  { "Keyword": "Guitar Hero", "PeakYear": 2008, "ImageURL":"https://thumb.wikimedia.org/wikipedia/en/thumb/2/2d/Guitarhero-cover.jpg/250px-Guitarhero-cover.jpg?utm_source=en.wikipedia.org\u0026utm_campaign=parser\u0026utm_content=thumbnail"}
]
```

## Tech stack

C#, .NET, Blazor WebAssembly, [NES.css](https://github.com/nostalgic-css/NES.css) for the retro UI, SerpApi (Google Trends), Gemini API via [Google_GenerativeAI](https://github.com/gunpal5/Google_GenerativeAI) for keyword generation, hosted on Vercel.

## Acknowledgements

- Trend data from Google Trends via [SerpApi](https://serpapi.com/).
- Gemini access via [Google_GenerativeAI](https://github.com/gunpal5/Google_GenerativeAI) by gunpal5 (MIT).
- UI styling by [NES.css](https://github.com/nostalgic-css/NES.css) (MIT).
- Font: [Press Start 2P](https://fonts.google.com/specimen/Press+Start+2P) by CodeMan38 (SIL Open Font License).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
