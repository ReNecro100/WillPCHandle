using SteamStorefrontAPI;
using SteamStorefrontAPI.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WillPC;

class AppCardInfo
{
    public int Id; //Айдишник отображаемого на главном экране приложения
    public string Name; //Название приложения
    public string Image; //Превьюшка приложения
    public string Genre; //Жанр игры

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
    public string HeaderImage; //Главный скрин
    public string Name; //Название
    public string Description; //Описание
    public string MinimalRequirements; //Минимальные требования
    public string RecommendedRequirements; //Рекомендованные требования
    public List<Screenshot> Screenshots; //Скрины
    public AppTotalInfo(string headerImage, string name, string description, string minimalRequirements, string recommendedRequirements, List<Screenshot> screenshots)
    {
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
        try
        {
            FeaturedApps featured = await Featured.GetAsync();
            List<AppInfo> apps = featured.FeaturedWin;

            List<AppCardInfo> featuredGames = new List<AppCardInfo>();
            foreach (var item in apps)
            {
                SteamApp app = await AppDetails.GetAsync(item.Id);
                AppCardInfo featuredGame = new AppCardInfo(item.Id, item.Name, item.LargeCapsuleImage, app.Genres[0].ToString());
                featuredGames.Add(featuredGame);
            }
            ManagerJSON.Serialize<AppCardInfo>(featuredGames, "cache/featuredGames.json");
            return featuredGames;
        }
        catch
        {
            return ManagerJSON.DeSerialize<AppCardInfo>("cache/featuredGames.json");
        }
    }
    public async Task<AppTotalInfo> GetAppTotalInfo(int gameId)
    {
        SteamApp app = await AppDetails.GetAsync(gameId);
        AppTotalInfo game = new AppTotalInfo(
            app.HeaderImage,
            app.Name,
            app.ShortDescription,
            app.PcRequirements.Minimum,
            app.PcRequirements.Recommended,
            app.Screenshots
            );
        return game;
    }
}
