
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using Color = Avalonia.Media.Color;
using Image = Avalonia.Controls.Image;

namespace WillPC
{
    public partial class MainWindowTest : Window
    {
        public MainWindowTest()
        {
            InitializeComponent();
            ShowMainPage();
        }

        //Арсен, что это???
        private void OpenSteam(object sender, RoutedEventArgs e)
        {
            string url = "https://store.steampowered.com/";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        public async void ShowMainPage()
        {
            SteamInterations steamInterations = new SteamInterations();
            List<GameInfo> games = await steamInterations.GetFeaturedAppsList();
            foreach (var item in games)
            {
                Border gameCard = CreateGameCard(item.HeaderImage, item.Name, item.Description.Substring(0, 24) + "...", item.Genre, item.CompatibilityIndicator);
                GamesContainer.Children.Add(gameCard);
            }
        }
        public static Border CreateGameCard(string imagePath, string name, string genre, string description, string compatibilityIndicator)
        {
            // Основной Border карточки
            var card = new Border
            {
                Classes = { "GameCard" }
            };

            // Основной Grid (140px верхняя часть, остальное нижняя)
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition(140, GridUnitType.Pixel));
            mainGrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));

            // ===== ВЕРХНЯЯ ЧАСТЬ (Изображение) =====
            var imageBorder = new Border
            {
                CornerRadius = new CornerRadius(16, 16, 0, 0),
                ClipToBounds = true
            };

            var imagePanel = new Panel
            {
                Background = new SolidColorBrush(Color.FromRgb(31, 42, 56))
            };

            var image = new Image
            {
                Source = new Bitmap(imagePath), // или используй IImage, если через ресурсы
                Stretch = Stretch.UniformToFill,
                Name = "GameInfoHeaderImage"
            };

            imagePanel.Children.Add(image);
            imageBorder.Child = imagePanel;
            Grid.SetRow(imageBorder, 0);
            mainGrid.Children.Add(imageBorder);

            // ===== НИЖНЯЯ ЧАСТЬ (Текст и статус) =====
            var textBorder = new Border
            {
                Padding = new Thickness(12)
            };

            var textGrid = new Grid();
            textGrid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
            textGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            // Блок с описанием
            var infoStack = new StackPanel
            {
                Spacing = 2
            };

            var nameText = new TextBlock
            {
                Text = name,
                FontSize = 14,
                FontWeight = FontWeight.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                Name = "GameInfoName"
            };

            var genreText = new TextBlock
            {
                Text = genre,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(139, 148, 158)),
                Name = "GameInfoGenre"
            };

            var descriptionText = new TextBlock
            {
                Text = description,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(139, 148, 158)),
                TextTrimming = TextTrimming.CharacterEllipsis,
                Name = "GameInfoHeaderDescription"
            };

            infoStack.Children.Add(nameText);
            infoStack.Children.Add(genreText);
            infoStack.Children.Add(descriptionText);
            Grid.SetRow(infoStack, 0);
            textGrid.Children.Add(infoStack);

            // Строка статуса
            var statusGrid = new Grid();
            statusGrid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            statusGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            statusGrid.Margin = new Thickness(0, 8, 0, 0);

            var statusLabel = new TextBlock
            {
                Text = "Status Indicator",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(139, 148, 158)),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            Grid.SetColumn(statusLabel, 0);
            statusGrid.Children.Add(statusLabel);

            var statusIndicator = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = GetStatusColor(compatibilityIndicator),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Name = "GameInfoStatusIndicator"
            };
            Grid.SetColumn(statusIndicator, 1);
            statusGrid.Children.Add(statusIndicator);

            Grid.SetRow(statusGrid, 1);
            textGrid.Children.Add(statusGrid);

            textBorder.Child = textGrid;
            Grid.SetRow(textBorder, 1);
            mainGrid.Children.Add(textBorder);

            card.Child = mainGrid;

            // Сохраняем ссылки на элементы, которые могут понадобиться позже

            return card;
        }
      //  <Border Classes = "GameCard" >

      //              < Grid RowDefinitions="140, *">
						//<!-- Верхняя часть: Картинка -->
						//<Border Grid.Row="0" CornerRadius= "16,16,0,0" ClipToBounds= "True" >

      //                      < Panel Background= "#1F2A38" >

      //                          < Image Source= "D:\Olszewski\WillPCHandle\WillPC\pics\1234.png" Stretch= "UniformToFill" />

      //                      </ Panel >

      //                  </ Border >


      //                  < !--Нижняя часть: Текст -->
						//<Border Grid.Row= "1" Padding= "12" >

      //                      < Grid RowDefinitions= "*, Auto" >

      //                          < !--Блок с описанием -->
						//		<StackPanel Grid.Row= "0" Spacing= "2" >

      //                              < TextBlock Text= "Geometry Dash" FontSize= "14" FontWeight= "Bold" Foreground= "White" />

      //                              < TextBlock Text= "Indi" FontSize= "12" Foreground= "#8B949E" />

      //                              < TextBlock Text= "The best indi game in the word" FontSize= "11" Foreground= "#8B949E" TextTrimming= "CharacterEllipsis" />

      //                          </ StackPanel >


      //                          < !--Строка статуса -->
						//		<Grid Grid.Row= "1" ColumnDefinitions= "*, Auto" Margin= "0,8,0,0" >

      //                              < TextBlock Grid.Column= "0" Text= "Status Indicator" FontSize= "11" Foreground= "#8B949E" VerticalAlignment= "Center" />

      //                              < Ellipse Grid.Column= "1" Width= "8" Height= "8" Fill= "#2ECC71" VerticalAlignment= "Center" />

      //                          </ Grid >

      //                      </ Grid >

      //                  </ Border >

      //              </ Grid >

      //          </ Border >
        private static IBrush GetStatusColor(string compatibilityIndicator)
        {
            return compatibilityIndicator switch
            {
                "ЗЕЛЁНЫЙ" => new SolidColorBrush(Color.FromRgb(0, 255, 0)),
                "ЖЁЛТЫЙ" => new SolidColorBrush(Color.FromRgb(255, 255, 0)),
                "КРАСНЫЙ" => new SolidColorBrush(Color.FromRgb(255, 0, 0)),
                _ => new SolidColorBrush(Color.FromRgb(143, 143, 143))
            };
        }


    }
}