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
    public SteamInterations() {
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
                await GetGameInfo(item.Id);
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
            SteamApp app = await AppDetails.GetAsync(gameId);

            //Получение индикатора
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

            //Получение скринов
            List<string> screenshots = new List<string>();
            await GetAppScreenshots(app.HeaderImage, gameId, 0);
            for (int i = 0; i < 3; i++)
            {
                await GetAppScreenshots(app.Screenshots[i].PathFull, gameId, i+1);
                screenshots.Add($"cache/{gameId}_{i+1}.jpg");
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
        using var file = File.OpenWrite($"cache/{gameId}_{screenshotId}.jpg");

        await stream.CopyToAsync(file);
    }
}
