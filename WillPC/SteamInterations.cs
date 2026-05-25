using Avalonia.Controls;
using SteamStorefrontAPI;
using SteamStorefrontAPI.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WillPC;
using System.IO;
using System.Net.Http;
using System.Net;

class GameInfo
{
    public int Id { get; set; } //Айдишник отображаемого приложения
    public string HeaderImage { get; set; } //Главный скрин
    public string Name { get; set; } //Название
    public string Description { get; set; } //Описание
    public string Genre { get; set; } //Жанр игры
    public string MinimalRequirements { get; set; } //Минимальные требования
    public string RecommendedRequirements { get; set; } //Рекомендованные требования
    public string CompatibilityIndicator { get; set; } //Индикатор совместимости (ЗЕЛЁНЫЙ, ЖЁЛТЫЙ, КРАСНЫЙ, СЕРЫЙ)
    public List<string> Screenshots { get; set; } //Скрины
    public GameInfo(int id, 
        string headerImage, string name, string description, 
        string genre, string minimalRequirements, string recommendedRequirements, 
        string compatibilityIndicator, List<string> screenshots)
    {
        Id = id;
        HeaderImage = headerImage;
        Name = name;
        Description = description;
        Genre = genre;
        MinimalRequirements = minimalRequirements;
        RecommendedRequirements = recommendedRequirements;
        CompatibilityIndicator = compatibilityIndicator;
        Screenshots = screenshots;
    }
}
class SteamInterations
{
    public SteamInterations()
    {
        Directory.CreateDirectory("cache");
        Trace.Listeners.Add(new TextWriterTraceListener("log_steam.txt"));
        Trace.AutoFlush = true;
    }
    public async Task<List<GameInfo>> GetFeaturedAppsList()
    {
        //https://feed.nuget.org/packages/SteamStorefrontAPI
        List<GameInfo>? featuredGames = ManagerJSON.DeSerialize<GameInfo>("cache/gamesList.json");
        if (featuredGames is null)
        {
            FeaturedApps featured = await Featured.GetAsync();
            List<AppInfo> apps = featured.FeaturedWin;

            featuredGames = new List<GameInfo>();
            foreach (var item in apps)
            {
                featuredGames.Add(await GetGameInfo(item.Id));
            }
            RefreshFeaturedAppsList(featuredGames);
        }
        return featuredGames;
    }
    public void RefreshFeaturedAppsList(List<GameInfo> FeaturedAppsList)
    {
        ManagerJSON.Serialize<GameInfo>(FeaturedAppsList, "cache/gamesList.json");
    }
    public async Task<GameInfo> GetGameInfo(int gameId)
    {
        //List<GameInfo>? cachedGame = ManagerJSON.DeSerialize<GameInfo>($"cache/steamGame_{gameId}.json");
        try
        {
            foreach (var item in ManagerJSON.DeSerialize<GameInfo>("cache/gamesList.json"))
            {
                if (item.Id == gameId)
                {
                    return item;
                }
            }
            throw new Exception("not found");
        }
        catch
        {
            string headerImagePath = $"cache/{gameId}_0.jpg";
            SteamApp app = await AppDetails.GetAsync(gameId);

            //Получение индикатора
            HardWareInteractons hardWareInteractons = new HardWareInteractons();
            GigaChatInteractions gigaChatInteractions = new GigaChatInteractions();

            string minimalRequirements = Regex.Replace(app.PcRequirements.Minimum is null ? "No requirements" : app.PcRequirements.Minimum.Replace("<li>", "\n"), "<[^>]*>", "");
            string recommendedRequirements = Regex.Replace(app.PcRequirements.Recommended is null ? "No requirements" : app.PcRequirements.Recommended.Replace("<li>", "\n"), "<[^>]*>", "");
            string prompt = "Тебе нужно определить, подходит ли мой компьютер под минимальные или рекомендованные требования игры:" +
                $"Конфигурация моего компьютера: {hardWareInteractons.GetPCData(1)}" +
                $"Минимальные требования игры: {minimalRequirements}" +
                $"Рекомендованные требования игры: {recommendedRequirements}" +
                $"Формат ответа - ТОЛЬКО ОДНО из приведённых ниже четырёх слов (без дополнительного описания):" +
                $"ЗЕЛЁНЫЙ - совместимость и с минимальными, и с рекомендованными требованиями игры" +
                $"ЖЁЛТЫЙ - совместимость только с минимальными требованиями игры" +
                $"КРАСНЫЙ - отсутствие совместимости" +
                $"СЕРЫЙ - требования не заданы" +
                "Если отсутствуют либо минимальные, либо рекомендованные требования, оценку совместимости проводи по тем требованиям, которые имеются. В таком случае ЗЕЛЁНЫЙ будет означать совместимость, а КРАСНЫЙ - её отсутствие";
            Trace.WriteLine(prompt);
            string compatibilityIndicator = gigaChatInteractions.GetGigaChatResponse(prompt);

            //Получение скринов
            List<string> screenshots = new List<string>();

            var appScreenshots = app.Screenshots;
            int screenshotsCount = appScreenshots is null ? 0 : Math.Min(3, appScreenshots.Count);
            for (int i = 0; i < screenshotsCount; i++)
            {
                await GetAppScreenshots(app.Screenshots[i].PathFull, gameId, i + 1);
                screenshots.Add($"cache/{gameId}_{i + 1}.jpg");
            }

            GameInfo game = new GameInfo(
                    gameId,
                    $"cache/{gameId}_0.jpg",
                    app.Name,
                    app.ShortDescription,
                    app.Genres[0].ToString(),
                    Regex.Replace(app.PcRequirements.Minimum is null ? "No requirements" : app.PcRequirements.Minimum.Replace("<li>", "\n"), "<[^>]*>", ""),
                    Regex.Replace(app.PcRequirements.Recommended is null ? "No requirements" : app.PcRequirements.Recommended.Replace("<li>", "\n"), "<[^>]*>", ""),
                    compatibilityIndicator,
                    screenshots
                    );
             return game;
        }

    }
    public async Task GetAppScreenshots(string fileURL, int gameId, int screenshotId)
    {
        using var httpClient = new HttpClient();
        using var stream = await httpClient.GetStreamAsync(fileURL);
        using var file = File.Create($"cache/{gameId}_{screenshotId}.jpg");

        await stream.CopyToAsync(file);
    }
}
//=======
//            await GetAppScreenshots(appScreenshots![i].PathFull, gameId, i + 1);
//            screenshots.Add($"cache/{gameId}_{i + 1}.jpg");
//>>>>>>> GUI
//        }

//        while (screenshots.Count < 3)
//        {
//            screenshots.Add(headerImagePath);
//        }

//        AppTotalInfo game = new AppTotalInfo(
//            gameId,
//            headerImagePath,
//            app.Name,
//            app.ShortDescription,
//            CleanRequirements(app.PcRequirements?.Minimum),
//            CleanRequirements(app.PcRequirements?.Recommended),
//            screenshots
//            );
//        RefreshAppTotalInfo(game);
//        return game;
//    }


//    public string GetCompatibilityIndicator(AppTotalInfo app)
//    {
//        return GetCompatibilityIndicator(app.MinimalRequirements, app.RecommendedRequirements);
//    }

//    public string GetCompatibilityIndicator(string minimalRequirements, string recommendedRequirements)
//    {
//        if (IsRequirementsEmpty(minimalRequirements) && IsRequirementsEmpty(recommendedRequirements))
//        {
//            return "СЕРЫЙ";
//        }

//        try
//        {
//            HardWareInteractons hardWareInteractons = new HardWareInteractons();
//            GigaChatInteractions gigaChatInteractions = new GigaChatInteractions();

//            string compatibilityIndicator = gigaChatInteractions.GetGigaChatResponse(
//                "Тебе нужно определить, подходит ли мой компьютер под минимальные или рекомендованные требования игры.\n" +
//                "Отвечай только одним словом из списка: ЗЕЛЁНЫЙ, ЖЁЛТЫЙ, КРАСНЫЙ, СЕРЫЙ.\n\n" +
//                "ЗЕЛЁНЫЙ - компьютер подходит под рекомендованные требования.\n" +
//                "ЖЁЛТЫЙ - компьютер подходит только под минимальные требования.\n" +
//                "КРАСНЫЙ - компьютер не подходит даже под минимальные требования.\n" +
//                "СЕРЫЙ - требования игры не заданы или данных недостаточно.\n\n" +
//                $"Конфигурация моего компьютера:\n{hardWareInteractons.GetPCData(1)}\n" +
//                $"Минимальные требования игры:\n{minimalRequirements}\n" +
//                $"Рекомендованные требования игры:\n{recommendedRequirements}\n"
//            );

//            return NormalizeCompatibilityIndicator(compatibilityIndicator);
//        }
//        catch (Exception ex)
//        {
//            Trace.WriteLine(ex);
//            return "СЕРЫЙ";
//        }
//    }

//    public static string NormalizeCompatibilityIndicator(string? compatibilityIndicator)
//    {
//        string upper = (compatibilityIndicator ?? string.Empty).Trim().ToUpperInvariant();

//        if (upper.Contains("ЗЕЛ"))
//        {
//            return "ЗЕЛЁНЫЙ";
//        }

//        if (upper.Contains("ЖЕЛ") || upper.Contains("ЖЁЛ"))
//        {
//            return "ЖЁЛТЫЙ";
//        }

//        if (upper.Contains("КРАС"))
//        {
//            return "КРАСНЫЙ";
//        }

//        return "СЕРЫЙ";
//    }

//    private static string CleanRequirements(string? requirements)
//    {
//        if (string.IsNullOrWhiteSpace(requirements))
//        {
//            return "No requirements";
//        }

//        string text = WebUtility.HtmlDecode(requirements.Replace("<li>", "\n"));
//        return Regex.Replace(text, "<[^>]*>", "").Trim();
//    }

//    private static bool IsRequirementsEmpty(string? requirements)
//    {
//        return string.IsNullOrWhiteSpace(requirements) ||
//               requirements.Trim().Equals("No requirements", StringComparison.OrdinalIgnoreCase);
//    }
//}
