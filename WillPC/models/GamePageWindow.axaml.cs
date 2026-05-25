using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace WillPC
{
    public partial class GamePageWindow : Window
    {
        public GamePageWindow()
        {
            InitializeComponent();
            ShowAppTotalInfo();
        }
        public async void ShowAppTotalInfo()
        {
            SteamInterations steamInterations = new SteamInterations();
            GameInfo game = await steamInterations.GetGameInfo(3871390);
            AppTotalInfoName.Text = game.Name;
            AppTotalInfoDescription.Text = game.Description + '\n' + game.MinimalRequirements + '\n' + game.RecommendedRequirements;
            AppTotalInfoScreenshot1.Source = new Bitmap(game.Screenshots[0]);
            AppTotalInfoScreenshot2.Source = new Bitmap(game.Screenshots[1]);
            AppTotalInfoScreenshot3.Source = new Bitmap(game.Screenshots[2]);
            AppTotalInfoHeaderImage.Source = new Bitmap(game.HeaderImage);
        }
    }
}