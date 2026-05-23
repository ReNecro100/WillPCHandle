using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace WillPC
{
    public partial class MainWindow : Window
    {
        private readonly SteamInterations _steamInterations = new();
        private readonly HardWareInteractons _hardWareInteractons = new();

        private readonly GamePreview[] _games =
        [
            new GamePreview(730, "Командный шутер"),
            new GamePreview(570, "MOBA"),
            new GamePreview(413150, "Фермерская RPG")
        ];

        public MainWindow()
        {
            InitializeComponent();
            _ = ShowMainPageCardsAsync();
        }

        private async Task ShowMainPageCardsAsync()
        {
            await RefreshPcInfoAsync();

            await LoadGameCardAsync(
                GameCard1,
                GameImage1,
                GameTitle1,
                GameGenre1,
                GameDescription1,
                GameIndicator1,
                GameIndicatorText1,
                _games[0]);

            await LoadGameCardAsync(
                GameCard2,
                GameImage2,
                GameTitle2,
                GameGenre2,
                GameDescription2,
                GameIndicator2,
                GameIndicatorText2,
                _games[1]);

            await LoadGameCardAsync(
                GameCard3,
                GameImage3,
                GameTitle3,
                GameGenre3,
                GameDescription3,
                GameIndicator3,
                GameIndicatorText3,
                _games[2]);
        }

        private async Task LoadGameCardAsync(
            Border card,
            Image image,
            TextBlock title,
            TextBlock genre,
            TextBlock description,
            Ellipse indicator,
            TextBlock indicatorText,
            GamePreview preview)
        {
            card.Tag = preview.Id;
            title.Text = "Загрузка...";
            genre.Text = preview.Genre;
            description.Text = "Steam отдает данные по AppId " + preview.Id;
            SetCompatibilityIndicator(indicator, indicatorText, "СЕРЫЙ", "Проверка...");

            try
            {
                AppTotalInfo game = await _steamInterations.GetAppTotalInfo(preview.Id);

                title.Text = game.Name;
                description.Text = ShortenText(game.Description, 170);

                if (!string.IsNullOrWhiteSpace(game.HeaderImage) && File.Exists(game.HeaderImage))
                {
                    image.Source = new Bitmap(game.HeaderImage);
                }

                string compatibility = await Task.Run(() => _steamInterations.GetCompatibilityIndicator(game));
                SetCompatibilityIndicator(indicator, indicatorText, compatibility);
            }
            catch (Exception ex)
            {
                title.Text = "Ошибка загрузки";
                description.Text = ex.Message;
                SetCompatibilityIndicator(indicator, indicatorText, "СЕРЫЙ", "Нет данных");
            }
        }

        private async void RefreshPcInfo(object? sender, RoutedEventArgs e)
        {
            await RefreshPcInfoAsync(true);
            _ = ShowMainPageCardsAsync();
        }

        private async Task RefreshPcInfoAsync(bool forceUpdate = false)
        {
            PcInfoText.Text = "Читаю конфигурацию ПК...";

            try
            {
                string pcData = await Task.Run(() => _hardWareInteractons.GetPCData(forceUpdate));
                PcInfoText.Text = pcData;
            }
            catch (Exception ex)
            {
                PcInfoText.Text = "Не удалось прочитать данные ПК: " + ex.Message;
            }
        }

        private void OpenGameCard(object? sender, PointerPressedEventArgs e)
        {
            if (sender is not Border { Tag: int appId })
            {
                return;
            }

            var gamePageWindow = new GamePageWindow(appId);
            if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = gamePageWindow;
            }

            gamePageWindow.Show();
            Close();
        }

        private void OpenSteam(object? sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://store.steampowered.com/",
                UseShellExecute = true
            });
        }

        private static void SetCompatibilityIndicator(
            Ellipse indicator,
            TextBlock label,
            string compatibility,
            string? displayText = null)
        {
            string normalized = SteamInterations.NormalizeCompatibilityIndicator(compatibility);
            label.Text = displayText ?? normalized;

            indicator.Fill = normalized switch
            {
                "ЗЕЛЁНЫЙ" => Brush.Parse("#2BD66F"),
                "ЖЁЛТЫЙ" => Brush.Parse("#FFD166"),
                "КРАСНЫЙ" => Brush.Parse("#FF4D4D"),
                _ => Brush.Parse("#7E8A96")
            };
        }

        private static string ShortenText(string? text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "Описание в Steam пока не найдено.";
            }

            string cleanText = text.Replace("\r", " ").Replace("\n", " ").Trim();
            return cleanText.Length <= maxLength
                ? cleanText
                : cleanText[..maxLength].TrimEnd() + "...";
        }

        private sealed record GamePreview(int Id, string Genre);
    }
}
