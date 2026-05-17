using SteamStorefrontAPI;
using SteamStorefrontAPI.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WillPC;

class SteamInterations
{
    public SteamInterations() {
        Trace.Listeners.Add(new TextWriterTraceListener("log_steam.txt"));
        Trace.AutoFlush = true;
    }
    public async Task<List<string>> GetFeaturedAppsListAsync()
    {
        //https://feed.nuget.org/packages/SteamStorefrontAPI
        FeaturedApps featured = await Featured.GetAsync("DE");
        //foreach (var item in featured.FeaturedWin)
        //Полезные поля:
        //Id
        //Name
        //HeaderImage
        //LargeCapsuleImage
        //SmallCapsuleImage
        List<string> games = new List<string>();
        foreach (var item in featured.FeaturedWin)
        {
            SteamApp game = await AppDetails.GetAsync(item.Id);
            //Полезные поля:
            //screenshots
            string reqs = game.PcRequirements==null ? "No requirements" : game.PcRequirements.Minimum + game.PcRequirements.Recommended;
            string gameInfo = item.Name + "\n" + game.ShortDescription + "\n" + reqs;
            games.Add(gameInfo);
            Trace.WriteLine(gameInfo);
        }
        return games;
        //Console.WriteLine($"Игра: {game.Name}");
        //Console.WriteLine($"Минимальные требования: {game.PcRequirements?.Minimum}");
        //Console.WriteLine($"Рекомендуемые требования: {game.PcRequirements?.Recommended}");

        //// Также доступны требования для других платформ
        //Console.WriteLine($"macOS требования: {game.MacRequirements?.Minimum}");
    }
}
