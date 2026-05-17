using SteamStorefrontAPI;
using SteamStorefrontAPI.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SteamInterations
{
    public async Task<List<string>> GetGameRequirementsWithPackageAsync()
    {
        //// Получаем данные об игре
        //SteamApp game = await AppDetails.GetAsync(appId);

        // Получить список популярных игр
        

        FeaturedApps featured = await Featured.GetAsync();
        List<string> games = new List<string>();
        foreach (var item in featured.FeaturedWin)
        {
            games.Add(Convert.ToString(item.Id));
        }
        return games;
        //Console.WriteLine($"Игра: {game.Name}");
        //Console.WriteLine($"Минимальные требования: {game.PcRequirements?.Minimum}");
        //Console.WriteLine($"Рекомендуемые требования: {game.PcRequirements?.Recommended}");

        //// Также доступны требования для других платформ
        //Console.WriteLine($"macOS требования: {game.MacRequirements?.Minimum}");
    }
}
