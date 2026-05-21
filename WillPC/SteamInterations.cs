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

                HardWareInteractons hardWareInteractons = new HardWareInteractons();
                GigaChatInteractions gigaChatInteractions = new GigaChatInteractions();
                
                string minimalRequirements = Regex.Replace(app.PcRequirements.Minimum is null ? "No requirements" : app.PcRequirements.Minimum.Replace("<li>", "\n"), "<[^>]*>", "");
                string recommendedRequirements = Regex.Replace(app.PcRequirements.Recommended is null ? "No requirements" : app.PcRequirements.Recommended.Replace("<li>", "\n"), "<[^>]*>", "");
                string compatibilityIndicator = gigaChatInteractions.GetGigaChatResponse(
                    "Тебе нужно определить, подходит ли мой компьютер под минимальные или рекомендованные требования игры:\n" +
                    $"Конфигурация моего компьютера: {hardWareInteractons.GetPCData()}\n" +
                    $"Минимальные требования игры: {minimalRequirements}\n" +
                    $"Рекомендованные требования игры: {recommendedRequirements}\n" +
                    $"Формат ответа - только одно из четырёх слов:\n" +
                    $"ЗЕЛЁНЫЙ - совместимость и с минимальными, и с рекомендованными требованиями игры\n" +
                    $"ЖЁЛТЫЙ - совместимость только с минимальными требованиями игры\n" +
                    $"КРАСНЫЙ - отсутствие совместимости\n" +
                    $"СЕРЫЙ - требования не заданы" +
                    "Если отсутствуют либо минимальные, либо рекомендованные требования, оценку совместимости проводи по тем требованиям, которые имеются. В таком случае ЗЕЛЁНЫЙ будет означать совместимость, а КРАСНЫЙ - её отсутствие\n"
                );

                AppCardInfo featuredGame = new AppCardInfo(
                    item.Id, 
                    item.Name, 
                    item.LargeCapsuleImage, 
                    app.Genres[0].ToString(),
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
        try
        {
            return ManagerJSON.DeSerialize<AppTotalInfo>($"cache/steamGame_{gameId}.json")[0];
        }
        catch
        {
            SteamApp app = await AppDetails.GetAsync(gameId);
            List<string> screenshots = new List<string>();
            string headerImagePath = $"cache/{gameId}_0.jpg";
            await GetAppScreenshots(app.HeaderImage, gameId, 0);

            int screenshotsCount = app.Screenshots is null ? 0 : Math.Min(3, app.Screenshots.Count);
            for (int i = 0; i < screenshotsCount; i++)
            {
                await GetAppScreenshots(app.Screenshots[i].PathFull, gameId, i+1);
                screenshots.Add($"cache/{gameId}_{i+1}.jpg");
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
                Regex.Replace(app.PcRequirements.Minimum is null ? "No requirements" : app.PcRequirements.Minimum.Replace("<li>", "\n"), "<[^>]*>", ""),
                Regex.Replace(app.PcRequirements.Recommended is null ? "No requirements" : app.PcRequirements.Recommended.Replace("<li>", "\n"), "<[^>]*>", ""),
                screenshots
                );
            RefreshAppTotalInfo(game);
            return game;
        }
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
}
