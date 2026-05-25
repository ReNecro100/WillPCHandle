using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WillPC
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
            ShowPCData();
        }
        public void ShowPCData()
        {
            SteamInterations steamInterations = new SteamInterations();
            HardWareInteractons hardWareInteractons = new HardWareInteractons();

            PCData.Text = hardWareInteractons.GetPCData(2);
        }
    }
}