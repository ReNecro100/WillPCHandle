using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

class HardWareInteractons
{
    public static string GetPCData()
    {
        string PCdata = "";
        ManagementObjectSearcher cpu = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
        ManagementObjectSearcher gpu = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
        ManagementObjectSearcher hdd = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");

        foreach (ManagementObject c in cpu.Get())
        {
            PCdata = "Процессор: " + c["Name"] + '\n' + "Количество ядер: " + c["NumberOfCores"] + '\n' + "Частота: " + c["MaxClockSpeed"] + " ГГц" + '\n';
        }
        foreach (ManagementObject g in gpu.Get())
        {
            PCdata += "Видеокарта: " + g["Name"] + '\n' + "Видеопамять: " + Math.Round(Convert.ToInt32(g["AdapterRAM"]) / Math.Pow(2, 30)) + " Гб" + '\n'; //Name, Caption, VideoProcessor
        }
        foreach (ManagementObject h in hdd.Get())
        {
            PCdata += $"Место на диске {h["Name"]} " + Math.Round(Convert.ToInt64(h["FreeSpace"]) / Math.Pow(2, 30)) + " Гб" + '\n'; //Name, Caption, VideoProcessor
        }
        //Для получения информации о жестких дисках(SELECT * FROM Win32_LogicalDisk)

        PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        PCdata += "Свободная оперативная память: " + ramCounter.NextValue() + " МБ" + '\n';
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
        foreach (ManagementObject os in searcher.Get())
        {
            PCdata += RuntimeInformation.OSDescription;
        }
        return PCdata;
    }
}
