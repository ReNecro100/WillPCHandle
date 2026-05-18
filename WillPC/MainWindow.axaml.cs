using Avalonia.Controls;
using System.Collections.Generic;

namespace WillPC
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Yes();
            GetGigachatResponse();
        }
        public async void Yes()
        {
            HardWareInteractons hardWareInteractons = new HardWareInteractons();
            Dat.Text = hardWareInteractons.GetPCData();
            SteamInterations steamInterations = new SteamInterations();
            List<AppCardInfo> featuredGames = await steamInterations.GetFeaturedAppsList();
            List<AppTotalInfo> gamesInfos = new List<AppTotalInfo>();
            foreach (var item in featuredGames)
            {
                AppTotalInfo app = await steamInterations.GetAppTotalInfo(item.Id);
                Response.Text += app.Name + "   - - - >   " + app.Description + '\n';
            }
        }
        public void GetGigachatResponse()
        {
            GigaChatInteractions gigaChatInteractions = new GigaChatInteractions();
            //Response.Text = gigaChatInteractions.GetGigaChatResponse("Почему трава красная?");
        }

    }
}
