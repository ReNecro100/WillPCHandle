using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;

namespace WillPC
{
    public partial class MainWindowTest : Window
    {
        public MainWindowTest()
        {
            InitializeComponent();
        }


        private void OpenSteam(object sender, RoutedEventArgs e)
        {
            string url = "https://store.steampowered.com/";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
       

        }
    }
