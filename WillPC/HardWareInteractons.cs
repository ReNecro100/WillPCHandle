using Avalonia.Controls.Shapes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;

class HardWareInteractons
{
    private string PCdata = string.Empty;

    public string GetPCData(bool toUpdate = false)
    {
<<<<<<< HEAD
        try
=======
        Directory.CreateDirectory("cache");
        PCdata = File.Exists("cache/PCdata.txt") ? File.ReadAllText("cache/PCdata.txt") : string.Empty;

        if (PCdata.Length < 3 || toUpdate)
>>>>>>> GUI
        {
            PCdata = File.ReadAllText("cache/PCdata.txt");
        }
        catch
        {
            PCdata = "a";
            if (PCdata.Length < 3 || toUpdate)
            {
                ManagementObjectSearcher cpu = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                ManagementObjectSearcher gpu = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                ManagementObjectSearcher hdd = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");

<<<<<<< HEAD
                foreach (ManagementObject c in cpu.Get())
                {
                    PCdata = "Процессор: " + c["Name"] + '\n' + "Количество ядер: " + c["NumberOfCores"] + '\n' + "Частота: " + c["MaxClockSpeed"] + " ГГц" + '\n';
                }
                foreach (ManagementObject g in gpu.Get())
                {
                    PCdata += "Видеокарта: " + g["Name"] + '\n' + "Видеопамять: " + Math.Round(Convert.ToInt64(g["AdapterRAM"]) / Math.Pow(2, 30)) + " Гб" + '\n'; //Name, Caption, VideoProcessor
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
                    PCdata += RuntimeInformation.OSDescription + '\n';
                }
                PCdata += "Версия DirectX: " + GetDirectXVersion() + '\n';
                File.WriteAllText("cache/PCdata.txt", PCdata);
            }
=======
            foreach (ManagementObject c in cpu.Get())
            {
                PCdata = "Процессор: " + c["Name"] + '\n' +
                         "Количество ядер: " + c["NumberOfCores"] + '\n' +
                         "Частота: " + c["MaxClockSpeed"] + " ГГц" + '\n';
            }

            foreach (ManagementObject g in gpu.Get())
            {
                PCdata += "Видеокарта: " + g["Name"] + '\n' +
                          "Видеопамять: " + Math.Round(Convert.ToInt64(g["AdapterRAM"]) / Math.Pow(2, 30)) + " Гб" + '\n';
            }

            foreach (ManagementObject h in hdd.Get())
            {
                if (h["FreeSpace"] != null)
                {
                    PCdata += $"Место на диске {h["Name"]} " +
                              Math.Round(Convert.ToInt64(h["FreeSpace"]) / Math.Pow(2, 30)) +
                              " Гб" + '\n';
                }
            }

            // ПОЛНЫЙ ОБЪЕМ ОПЕРАТИВНОЙ ПАМЯТИ
            ManagementObjectSearcher ram = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");

            foreach (ManagementObject r in ram.Get())
            {
                PCdata += "Оперативная память: " +
                          Math.Round(Convert.ToDouble(r["TotalPhysicalMemory"]) / Math.Pow(2, 30), 2) +
                          " Гб" + '\n';
            }

            // СВОБОДНАЯ ОПЕРАТИВНАЯ ПАМЯТЬ
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            PCdata += "Свободная оперативная память: " + ramCounter.NextValue() + " МБ" + '\n';

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");

            foreach (ManagementObject os in searcher.Get())
            {
                PCdata += RuntimeInformation.OSDescription + '\n';
            }

            PCdata += "Версия DirectX: " + GetDirectXVersion() + '\n';

            File.WriteAllText("cache/PCdata.txt", PCdata);
>>>>>>> GUI
        }

        return PCdata;
    }

    public static string GetDirectXVersion()
    {
        string registryPath = @"SOFTWARE\Microsoft\DirectX";
        string registryPath32On64 = @"SOFTWARE\Wow6432Node\Microsoft\DirectX";

        using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(registryPath) ??
                                 Registry.LocalMachine.OpenSubKey(registryPath32On64))
        {
            if (key != null)
            {
                string? version = key.GetValue("Version") as string;

                if (!string.IsNullOrEmpty(version))
                {
                    return ParseDirectXVersion(version);
                }
            }
        }

        return "Неизвестная версия";
    }

    private static string ParseDirectXVersion(string registryVersion)
    {
        var parts = registryVersion.Split('.');

        if (parts.Length >= 2)
        {
            int major = int.Parse(parts[1]);

            switch (major)
            {
                case 2: return "DirectX 2.0";
                case 3: return "DirectX 3.0";
                case 4: return "DirectX 4.0";
                case 5: return "DirectX 5.0";
                case 6: return "DirectX 6.0";
                case 7: return "DirectX 7.0";

                case 8:
                    if (parts.Length >= 3 && parts[2] == "01")
                        return "DirectX 8.1";

                    return "DirectX 8.0";

                case 9:
                    if (parts.Length >= 4)
                    {
                        int build = int.Parse(parts[3]);

                        if (build <= 0900) return "DirectX 9.0";
                        if (build <= 0901) return "DirectX 9.0a";
                        if (build <= 0902) return "DirectX 9.0b";

                        return "DirectX 9.0c";
                    }

                    return "DirectX 9.0";

                default:
                    return $"DirectX {major} (оценка)";
            }
        }

        return "Неизвестная версия";
    }
}
