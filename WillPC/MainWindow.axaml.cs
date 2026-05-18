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
            foreach (var item in await steamInterations.GetFeaturedAppsList())
            {
                Response.Text += item.Name;
            }
        }
        public void GetGigachatResponse()
        {
            GigaChatInteractions gigaChatInteractions = new GigaChatInteractions();
            //Response.Text = gigaChatInteractions.GetGigaChatResponse("Почему трава красная?");
        }

    }
}
