using Avalonia.Controls;
using System;
using System.Management;

namespace WillPC
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Yes();
        }
        public void Yes()
        {
            ManagementObjectSearcher cpu = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            ManagementObjectSearcher gpu = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");

            foreach (ManagementObject c in cpu.Get())
            {
                Dat.Text = "Процессор: " + c["Name"] + '\n' + "Количество ядер: " + c["NumberOfCores"] + "Частота: " + c["MaxClockSpeed"] + " МГц" +'\n';
            }
            foreach (ManagementObject g in gpu.Get())
            {
                Dat.Text += "Видеокарта: " + g["Name"] + '\n' + "Видеопамять: " + Math.Round(Convert.ToInt32(g["AdapterRAM"]) / Math.Pow(2, 30)) + " Гб"; //Name, Caption, VideoProcessor
            }
            //Для получения информации о жестких дисках(SELECT * FROM Win32_LogicalDisk)

            //PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            //Console.WriteLine("Свободная оперативная память: " + ramCounter.NextValue() + " МБ");
            //Console.ReadLine();
        }
    }
}
