using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Media.Transformation;
using Avalonia.Platform;
using Avalonia.Threading;

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
            try
            {
                SteamInterations steamInterations = new SteamInterations();
                AppTotalInfo game = await steamInterations.GetAppTotalInfo(2483190);
                AppTotalInfoName.Text = game.Name;
                AppTotalInfoDescription.Text = game.Description + '\n'+game.MinimalRequirements +'\n' +game.RecommendedRequirements;
                AppTotalInfoScreenshot1.Source = new Bitmap(game.Screenshots[0]);
                AppTotalInfoScreenshot2.Source = new Bitmap(game.Screenshots[1]);
                AppTotalInfoScreenshot3.Source = new Bitmap(game.Screenshots[2]);
                AppTotalInfoHeaderImage.Source = new Bitmap(game.HeaderImage);
            }
            catch (Exception ex)
            {
                File.WriteAllText("game_page_error.txt", ex.ToString());
                AppTotalInfoName.Text = "Ошибка загрузки";
                AppTotalInfoDescription.Text = ex.Message;
            }
        }
        //<Run Text = "� ���� ���������������� ������-�������������� ������ �� ������� ���� ��� ��������� ��������� �� ���� ���� ������������ �������� �������������, ������������ � ����� ������ ���������������� ���������� ��������. ����������� � ��������� �������� ����� ��-��� ��������, ������ ������� � ���� ���������, � ������ ����� ��������� ������� ������������ ������ � ���������� ��������" ></ Run >
        //                        < LineBreak />
        //                        < Run Text="��������� ����������:"/> 
        //                        <Run Text = "10 (64-���)" />
        //                        < LineBreak />
        //                        < Run Text ="���������: Intel Core i5-750 (2.6 ���) ��� AMD Phenom II X4 955 (3.2 ���)"/>
        //                        <LineBreak/>

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
        //                        <Run Text = "����������� ������: 4 �� ���" />
        //                        < LineBreak />
        //                        ����������: NVIDIA GeForce GTX 460 ��� AMD Radeon HD 5850 (1 �� VRAM)
        //                        <LineBreak/>
        //                        DirectX: ������ 11
        //                        <LineBreak/>
        //                        ����� �� �����: 30 �� ���������� ������������"
        //                        <LineBreak/>
    }
}
