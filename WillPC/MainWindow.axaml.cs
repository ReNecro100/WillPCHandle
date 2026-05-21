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
            List<AppCardInfo> featuredGames = await steamInterations.GetFeaturedAppsList();
            foreach (var item in featuredGames)
            {
                //Тут бери информацию о карточках и выводи её:
                //Name - Название приложения
                //Image - Превьюшка приложения
                //Genre - Жанр игры

                //CompatibilityIndicator - одно из этих четырёх слов:
                //- КРАСНЫЙ
                //- ЖЁЛТЫЙ
                //- ЗЕЛЁНЫЙ
                //- СЕРЫЙ
                //Тебе нужно  будет просто сделать так, чтобы индикатор подсвечивался соответствующим цветов. Я верю в тебя!!!1!1!!!!1!11!!!

                //Собственно говоря, тут пиши алгоритм для изменения цвета индикатора
            }
            Dat.Text = hardWareInteractons.GetPCData();
        }
        //А какие ещё должны быть функции?

    }
}
