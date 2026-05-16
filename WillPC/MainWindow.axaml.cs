using Avalonia.Controls;
using System;
using System.Diagnostics;
using System.Management;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WillPC
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Yes();
            GetGigachatResponse();
        }
        public void Yes()
        {
            ManagementObjectSearcher cpu = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            ManagementObjectSearcher gpu = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            ManagementObjectSearcher hdd = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");

            foreach (ManagementObject c in cpu.Get())
            {
                Dat.Text = "Процессор: " + c["Name"] + '\n' + "Количество ядер: " + c["NumberOfCores"] + '\n' + "Частота: " + c["MaxClockSpeed"] + " МГц" +'\n';
            }
            foreach (ManagementObject g in gpu.Get())
            {
                Dat.Text += "Видеокарта: " + g["Name"] + '\n' + "Видеопамять: " + Math.Round(Convert.ToInt32(g["AdapterRAM"]) / Math.Pow(2, 30)) + " Гб" + '\n'; //Name, Caption, VideoProcessor
            }
            foreach (ManagementObject h in hdd.Get())
            {
                Dat.Text += $"Место на диске {h["Name"]} " + Math.Round(Convert.ToInt64(h["FreeSpace"]) / Math.Pow(2, 30)) + " Гб" + '\n'; //Name, Caption, VideoProcessor
            }
            //Для получения информации о жестких дисках(SELECT * FROM Win32_LogicalDisk)

            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            Dat.Text+="Свободная оперативная память: " + ramCounter.NextValue() + " МБ" + '\n';
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                Dat.Text += RuntimeInformation.OSDescription;
            }
        }
        public void GetGigachatResponse()
        {
            GigaChatInteractions gigaChatInteractions = new GigaChatInteractions();
            Response.Text = "Token: ";
            Response.Text += gigaChatInteractions.GetGigaChatResponse("Дай одну коротку цитату Климента Римского");
        }
    }
}
