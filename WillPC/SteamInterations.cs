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

class AppCardInfo
{
    public int Id { get; set; } //Айдишник отображаемого на главном экране приложения
    public string Name { get; set; } //Название приложения
    public string Image { get; set; } //Превьюшка приложения
    public string Genre { get; set; } //Жанр игры
    public string CompatibilityIndicator { get; set; } //Индикатор совместимости (ЗЕЛЁНЫЙ, ЖЁЛТЫЙ, КРАСНЫЙ, СЕРЫЙ)

    public AppCardInfo(int id, string name, string image, string genre,string compatibilityIndicator)
    {
        Id = id;
        Name = name;
        Image = image;
        Genre = genre;
        CompatibilityIndicator = compatibilityIndicator;
    }
}
class AppTotalInfo
{
    public int Id { get; set; } //Айдишник отображаемого приложения
    public string HeaderImage { get; set; } //Главный скрин
    public string Name { get; set; } //Название
    public string Description { get; set; } //Описание
    public string MinimalRequirements { get; set; } //Минимальные требования
    public string RecommendedRequirements { get; set; } //Рекомендованные требования
    public List<string> Screenshots { get; set; } //Скрины
    public AppTotalInfo(int id, string headerImage, string name, string description, string minimalRequirements, string recommendedRequirements, List<string> screenshots)
    {
        Id = id;
        HeaderImage = headerImage;
        Name = name;
        Description = description;
        MinimalRequirements = minimalRequirements;
        RecommendedRequirements = recommendedRequirements;
        Screenshots = screenshots;
    }
}
class SteamInterations
{
    public SteamInterations() {
        Directory.CreateDirectory("cache");
        Trace.Listeners.Add(new TextWriterTraceListener("log_steam.txt"));
        Trace.AutoFlush = true;
    }
    public async Task<List<AppCardInfo>> GetFeaturedAppsList()
    {
        //https://feed.nuget.org/packages/SteamStorefrontAPI
        List<AppCardInfo>? featuredGames = ManagerJSON.DeSerialize<AppCardInfo>("cache/featuredGames.json");
        if (featuredGames is null)
        {
            FeaturedApps featured = await Featured.GetAsync();
            List<AppInfo> apps = featured.FeaturedWin;

            featuredGames = new List<AppCardInfo>();
            foreach (var item in apps)
            {
                SteamApp app = await AppDetails.GetAsync(item.Id);

                string minimalRequirements = CleanRequirements(app.PcRequirements?.Minimum);
                string recommendedRequirements = CleanRequirements(app.PcRequirements?.Recommended);
                string compatibilityIndicator = GetCompatibilityIndicator(minimalRequirements, recommendedRequirements);

                AppCardInfo featuredGame = new AppCardInfo(
                    item.Id, 
                    item.Name, 
                    item.LargeCapsuleImage, 
                    app.Genres is not null && app.Genres.Count > 0 ? app.Genres[0].ToString() : "Game",
                    compatibilityIndicator);
                featuredGames.Add(featuredGame);
            }
            RefreshFeaturedAppsList(featuredGames);
        }
        return featuredGames;
    }
    public void RefreshFeaturedAppsList(List<AppCardInfo> FeaturedAppsList)
    {
        ManagerJSON.Serialize<AppCardInfo>(FeaturedAppsList, "cache/featuredGames.json");
    }
    public async Task<AppTotalInfo> GetAppTotalInfo(int gameId)
    {
        List<AppTotalInfo>? cachedGame = ManagerJSON.DeSerialize<AppTotalInfo>($"cache/steamGame_{gameId}.json");
        if (cachedGame is not null && cachedGame.Count > 0)
        {
            return cachedGame[0];
        }

        SteamApp app = await AppDetails.GetAsync(gameId);
        List<string> screenshots = new List<string>();
        string headerImagePath = $"cache/{gameId}_0.jpg";
        await GetAppScreenshots(app.HeaderImage, gameId, 0);

        var appScreenshots = app.Screenshots;
        int screenshotsCount = appScreenshots is null ? 0 : Math.Min(3, appScreenshots.Count);
        for (int i = 0; i < screenshotsCount; i++)
        {
            await GetAppScreenshots(appScreenshots![i].PathFull, gameId, i + 1);
            screenshots.Add($"cache/{gameId}_{i + 1}.jpg");
        }

        while (screenshots.Count < 3)
        {
            screenshots.Add(headerImagePath);
        }

        AppTotalInfo game = new AppTotalInfo(
            gameId,
            headerImagePath,
            app.Name,
            app.ShortDescription,
            CleanRequirements(app.PcRequirements?.Minimum),
            CleanRequirements(app.PcRequirements?.Recommended),
            screenshots
            );
        RefreshAppTotalInfo(game);
        return game;
    }
    public void RefreshAppTotalInfo(AppTotalInfo app)
    {
        ManagerJSON.Serialize<AppTotalInfo>(new List<AppTotalInfo> () { app }, $"cache/steamGame_{app.Id}.json");
    }
    public async Task GetAppScreenshots(string fileURL, int gameId, int screenshotId)
    {
        using var httpClient = new HttpClient();
        using var stream = await httpClient.GetStreamAsync(fileURL);
        using var file = File.Create($"cache/{gameId}_{screenshotId}.jpg");

        await stream.CopyToAsync(file);
    }

    public string GetCompatibilityIndicator(AppTotalInfo app)
    {
        return GetCompatibilityIndicator(app.MinimalRequirements, app.RecommendedRequirements);
    }

    public string GetCompatibilityIndicator(string minimalRequirements, string recommendedRequirements)
    {
        if (IsRequirementsEmpty(minimalRequirements) && IsRequirementsEmpty(recommendedRequirements))
        {
            return "СЕРЫЙ";
        }

        try
        {
            HardWareInteractons hardWareInteractons = new HardWareInteractons();
            GigaChatInteractions gigaChatInteractions = new GigaChatInteractions();

            string compatibilityIndicator = gigaChatInteractions.GetGigaChatResponse(
                "Тебе нужно определить, подходит ли мой компьютер под минимальные или рекомендованные требования игры.\n" +
                "Отвечай только одним словом из списка: ЗЕЛЁНЫЙ, ЖЁЛТЫЙ, КРАСНЫЙ, СЕРЫЙ.\n\n" +
                "ЗЕЛЁНЫЙ - компьютер подходит под рекомендованные требования.\n" +
                "ЖЁЛТЫЙ - компьютер подходит только под минимальные требования.\n" +
                "КРАСНЫЙ - компьютер не подходит даже под минимальные требования.\n" +
                "СЕРЫЙ - требования игры не заданы или данных недостаточно.\n\n" +
                $"Конфигурация моего компьютера:\n{hardWareInteractons.GetPCData()}\n" +
                $"Минимальные требования игры:\n{minimalRequirements}\n" +
                $"Рекомендованные требования игры:\n{recommendedRequirements}\n"
            );

            return NormalizeCompatibilityIndicator(compatibilityIndicator);
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex);
            return "СЕРЫЙ";
        }
    }

    public static string NormalizeCompatibilityIndicator(string? compatibilityIndicator)
    {
        string upper = (compatibilityIndicator ?? string.Empty).Trim().ToUpperInvariant();

        if (upper.Contains("ЗЕЛ"))
        {
            return "ЗЕЛЁНЫЙ";
        }

        if (upper.Contains("ЖЕЛ") || upper.Contains("ЖЁЛ"))
        {
            return "ЖЁЛТЫЙ";
        }

        if (upper.Contains("КРАС"))
        {
            return "КРАСНЫЙ";
        }

        return "СЕРЫЙ";
    }

    private static string CleanRequirements(string? requirements)
    {
        if (string.IsNullOrWhiteSpace(requirements))
        {
            return "No requirements";
        }

        string text = WebUtility.HtmlDecode(requirements.Replace("<li>", "\n"));
        return Regex.Replace(text, "<[^>]*>", "").Trim();
    }

    private static bool IsRequirementsEmpty(string? requirements)
    {
        return string.IsNullOrWhiteSpace(requirements) ||
               requirements.Trim().Equals("No requirements", StringComparison.OrdinalIgnoreCase);
    }
}
