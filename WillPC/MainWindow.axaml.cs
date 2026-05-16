using Avalonia.Controls;
using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace WillPC
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Yes();
            GetGigachatResponse();
        }
        public void Yes()
        {
            Dat.Text = HardWareInteractons.GetPCData();
        }
        public void GetGigachatResponse()
        {
            GigaChatInteractions gigaChatInteractions = new GigaChatInteractions();
            //Response.Text = "Ответ нейронки: ";
            //Response.Text += gigaChatInteractions.GetGigaChatResponse("Просто напиши АГА КОНЕЧНО");
        }
    }
}
