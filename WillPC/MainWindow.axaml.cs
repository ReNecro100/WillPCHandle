using Avalonia.Controls;

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
            Dat.Text = HardWareInteractons.GetPCData();
            SteamInterations steamInterations = new SteamInterations();
            foreach (var item in await steamInterations.GetFeaturedAppsListAsync())
            {
                Response.Text += item + '\n';
            }
        }
        public void GetGigachatResponse()
        {
            GigaChatInteractions gigaChatInteractions = new GigaChatInteractions();
            //Response.Text = "Ответ нейронки: ";
            //Response.Text += gigaChatInteractions.GetGigaChatResponse("Просто напиши АГА КОНЕЧНО");
        }
        
    }
}
