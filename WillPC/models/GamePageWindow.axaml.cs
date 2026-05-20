using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;

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
            AppTotalInfo game = await steamInterations.GetAppTotalInfo(1602000);
            AppTotalInfoName.Text = game.Name;
            AppTotalInfoDescription.Text = game.Description + '\n'+game.MinimalRequirements +'\n' +game.RecommendedRequirements;
        }
        //<Run Text = "В этом высокоскоростном научно-фантастическом шутере от первого лица вам предстоит примерить на себя роль оперативника элитного подразделения, заброшенного в самое сердце полуразрушенного мегаполиса будущего. Эксперимент с квантовой энергией вышел из-под контроля, открыв разломы в иные измерения, и теперь улицы наводнили легионы безжалостных тварей и искаженных мутантов" ></ Run >
        //                        < LineBreak />
        //                        < Run Text="Системные требования:"/> 
        //                        <Run Text = "10 (64-бит)" />
        //                        < LineBreak />
        //                        < Run Text ="Процессор: Intel Core i5-750 (2.6 ГГц) или AMD Phenom II X4 955 (3.2 ГГц)"/>
        //                        <LineBreak/>
        //                        <Run Text = "Оперативная память: 4 ГБ ОЗУ" />
        //                        < LineBreak />
        //                        Видеокарта: NVIDIA GeForce GTX 460 или AMD Radeon HD 5850 (1 ГБ VRAM)
        //                        <LineBreak/>
        //                        DirectX: Версия 11
        //                        <LineBreak/>
        //                        Место на диске: 30 ГБ свободного пространства"
        //                        <LineBreak/>
    }
}