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

class AppCardInfo
{
    public int Id { get; set; } //Айдишник отображаемого на главном экране приложения
    public string Name { get; set; } //Название приложения
    public string Image { get; set; } //Превьюшка приложения
    public string Genre { get; set; } //Жанр игры

    public AppCardInfo(int id, string name, string image, string genre)
    {
        Id = id;
        Name = name;
        Image = image;
        Genre = genre;
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
    public List<Screenshot> Screenshots { get; set; } //Скрины
    public AppTotalInfo(int id, string headerImage, string name, string description, string? minimalRequirements, string? recommendedRequirements, List<Screenshot> screenshots)
    {
        Id = id;
        HeaderImage = headerImage;
        Name = name;
        Description = description;
        if (minimalRequirements is null) MinimalRequirements = "No requirements"; else MinimalRequirements = minimalRequirements;
        if (recommendedRequirements is null) RecommendedRequirements = "No requirements"; else RecommendedRequirements = recommendedRequirements;
        Screenshots = screenshots;
    }
}
class SteamInterations
{
    public SteamInterations() {
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
                AppCardInfo featuredGame = new AppCardInfo(item.Id, item.Name, item.LargeCapsuleImage, app.Genres[0].ToString());
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
            AppTotalInfo game = new AppTotalInfo(
                gameId,
                app.HeaderImage,
                app.Name,
                app.ShortDescription,
                Regex.Replace(app.PcRequirements.Minimum is null ? "No requirements" : app.PcRequirements.Minimum, "<[^>]*>", ""),
                Regex.Replace(app.PcRequirements.Recommended is null ? "No requirements" : app.PcRequirements.Recommended, "<[^>]*>", ""),
                app.Screenshots
                );
            RefreshAppTotalInfo(game);
            return game;
        }
    }
    public void RefreshAppTotalInfo(AppTotalInfo app)
    {
        ManagerJSON.Serialize<AppTotalInfo>(new List<AppTotalInfo> () { app }, $"cache/steamGame_{app.Id}.json");
    }
}
