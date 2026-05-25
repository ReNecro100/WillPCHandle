using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.Transformation;
using Avalonia.Threading;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WillPC
{
    public partial class GamePageWindow : Window
    {
        private readonly int _gameId;
        private readonly SteamInterations _steamInterations = new();

        public GamePageWindow() : this(730)
        {
        }

        public GamePageWindow(int gameId)
        {
            _gameId = gameId;
            InitializeComponent();
            _ = ShowAppTotalInfoAsync();
        }

        private async Task ShowAppTotalInfoAsync()
        {
            try
            {
                GameInfo game = await _steamInterations.GetGameInfo(_gameId);

                AppTotalInfoName.Text = game.Name;
                AppTotalInfoDescription.Text =
                    game.Description + "\n\n" +
                    "Минимальные требования:\n" + game.MinimalRequirements + "\n\n" +
                    "Рекомендованные требования:\n" + game.RecommendedRequirements;

                AppTotalInfoHeaderImage.Source = new Bitmap(game.HeaderImage);
                AppTotalInfoScreenshot1.Source = new Bitmap(game.Screenshots[0]);
                AppTotalInfoScreenshot2.Source = new Bitmap(game.Screenshots[1]);
                AppTotalInfoScreenshot3.Source = new Bitmap(game.Screenshots[2]);

                string compatibility = game.CompatibilityIndicator;
                SetCompatibilityBadge(compatibility);
            }
            catch (Exception ex)
            {
                File.WriteAllText("game_page_error.txt", ex.ToString());
                AppTotalInfoName.Text = "Ошибка загрузки";
                AppTotalInfoDescription.Text = ex.Message;
                SetCompatibilityBadge("СЕРЫЙ");
            }
        }

        private void BackToMain(object? sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = mainWindow;
            }

            mainWindow.Show();
            Close();
        }

        private void SetCompatibilityBadge(string compatibility)
        {
            string normalized = compatibility;
            CompatibilityText.Text = normalized;

            CompatibilityBadge.Background = normalized switch
            {
                "ЗЕЛЁНЫЙ" => Brush.Parse("#2BD66F"),
                "ЖЁЛТЫЙ" => Brush.Parse("#FFD166"),
                "КРАСНЫЙ" => Brush.Parse("#FF4D4D"),
                _ => Brush.Parse("#7E8A96")
            };

            CompatibilityText.Foreground = normalized == "ЖЁЛТЫЙ"
                ? Brush.Parse("#15212C")
                : Brushes.White;
        }

        private void OpenPhotoPreview(object? sender, PointerPressedEventArgs e)
        {
            if (sender is not Image image || image.Source is null)
            {
                return;
            }

            PhotoPreviewImage.Source = image.Source;
            PhotoPreviewOverlay.IsVisible = true;
            PhotoPreviewOverlay.Opacity = 0;
            PhotoPreviewCard.RenderTransform = TransformOperations.Parse("scale(0.92)");

            Dispatcher.UIThread.Post(() =>
            {
                PhotoPreviewOverlay.Opacity = 1;
                PhotoPreviewCard.RenderTransform = TransformOperations.Parse("scale(1)");
            });

            e.Handled = true;
        }

        private async void ClosePhotoPreview(object? sender, PointerPressedEventArgs e)
        {
            await HidePhotoPreview();
        }

        private void KeepPhotoPreviewOpen(object? sender, PointerPressedEventArgs e)
        {
            e.Handled = true;
        }

        private async void ClosePhotoPreviewButton(object? sender, RoutedEventArgs e)
        {
            await HidePhotoPreview();
        }

        private async Task HidePhotoPreview()
        {
            if (!PhotoPreviewOverlay.IsVisible)
            {
                return;
            }

            PhotoPreviewOverlay.Opacity = 0;
            PhotoPreviewCard.RenderTransform = TransformOperations.Parse("scale(0.92)");

            await Task.Delay(180);

            PhotoPreviewOverlay.IsVisible = false;
            PhotoPreviewImage.Source = null;
        }
    }
}
