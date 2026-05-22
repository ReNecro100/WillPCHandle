using Avalonia.Controls;
using System.Collections.Generic;

namespace WillPC
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowMainPageCards();
        }

        //Функции для кнопок:
        //
        //Для перехода на страницу с игрой (там функция инфу нужную возвращает):
        //await steamInterations.GetAppTotalInfo(item.Id);
        //
        //Для получения конфигурации ПК:
        //GetPCData(true)
        //
        //Ну, пользователя как-нибудь создашь
        //
        //Функция вывода карточек:
        public async void ShowMainPageCards()
        {
            SteamInterations steamInterations = new SteamInterations();
            HardWareInteractons hardWareInteractons = new HardWareInteractons();
            
            Dat.Text = hardWareInteractons.GetPCData(1);
        }
        //А какие ещё должны быть функции?

    }
}
