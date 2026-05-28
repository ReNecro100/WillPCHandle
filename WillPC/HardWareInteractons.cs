using Avalonia.Controls.Platform;
using Avalonia.Controls.Shapes;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

// 1. GPU
public class GPU
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string RAM { get; set; }

    public GPU() { }

    public GPU(int id, string name, string ram)
    {
        Id = id;
        Name = name;
        RAM = ram;
    }
    public override string ToString() => Name;
}

// 2. CPU
public class CPU
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Cores { get; set; }
    public string Speed { get; set; }

    public CPU() { }

    public CPU(int id, string name, int cores, string speed)
    {
        Id = id;
        Name = name;
        Cores = cores;
        Speed = speed;
    }
    public override string ToString() => Name;
}

// 3. DirectX
public class DirectX
{
    public int Id { get; set; }
    public string Name { get; set; }

    public DirectX() { }

    public DirectX(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public override string ToString() => Name;
}

// 4. OS
public class OS
{
    public int Id { get; set; }
    public string Name { get; set; }

    public OS() { }

    public OS(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public override string ToString() => Name;
}

// 5. Config
public class Config
{
    public int Id { get; set; }
    public GPU GPU { get; set; }
    public CPU CPU { get; set; }
    public string RAM { get; set; }
    public OS OS { get; set; }
    public DirectX DirectX { get; set; }
    public List<HDD> HDDS { get; set; }

    public Config() { }

    public Config(int id, GPU gpu, CPU cpu, string ram, OS os, DirectX directX, List<HDD> hDDS)
    {
        Id = id;
        GPU = gpu;
        CPU = cpu;
        RAM = ram;
        OS = os;
        DirectX = directX;
        HDDS = hDDS;
    }
}

// 6. HDD
public class HDD
{
    public string Name { get; set; }
    public string FreeSpace { get; set; }

    public HDD() { }

    public HDD(string name, string freeSpace)
    {
        Name = name;
        FreeSpace = freeSpace;
    }
}
class HardWareInteractons
{
    private string PCdata;
    public string GetPCData(int configId, bool toUpdate = false)
    {
        try {
            string sqlExpression = $"SELECT \r\n    config.id,\r\n    config.GPU,\r\n    config.CPU,\r\n    config.RAM,\r\n    config.OS,\r\n    config.DirectX,\r\n    cpu.id,\r\n    cpu.Name,\r\n    cpu.Cores,\r\n    cpu.Speed,\r\n    directX.id,\r\n    directX.Name,\r\n    gpu.id,\r\n    gpu.Name,\r\n    gpu.RAM,\r\n    os.id,\r\n    os.Name,\r\n    hdd.id,\r\n    hdd.Config,\r\n    hdd.Name,\r\n    hdd.FreeSpace\r\nFROM config\r\nLEFT JOIN cpu ON config.cpu = cpu.id\r\nLEFT JOIN gpu ON config.gpu = gpu.id\r\nLEFT JOIN directX ON config.directX = directX.id\r\nLEFT JOIN os ON config.os = os.id\r\nLEFT JOIN hdd ON hdd.config = config.id\r\nWHERE config.id = {configId};";
            using (var connection = new SqliteConnection("Data Source=data.db"))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(sqlExpression, connection);
                using (SqliteDataReader readerConfig = command.ExecuteReader())
                {
                    if (readerConfig.HasRows)
                    {
                        List<HDD> hdds = new List<HDD>();
                        Config pcConfig = new Config();
                        while (readerConfig.Read())
                        {
                            CPU cPU = new CPU(readerConfig.GetInt32(6), readerConfig.GetString(7), readerConfig.GetInt32(8), readerConfig.GetString(9));
                            DirectX directX = new DirectX(readerConfig.GetInt32(10), readerConfig.GetString(11));
                            GPU gPU = new GPU(readerConfig.GetInt32(2), readerConfig.GetString(13), readerConfig.GetString(14));
                            OS oS = new OS(readerConfig.GetInt32(15), readerConfig.GetString(16));
                            hdds.Add(new HDD(readerConfig.GetString(19), readerConfig.GetString(20)));
                            pcConfig = new Config(readerConfig.GetInt32(0), gPU, cPU, readerConfig.GetString(3), oS, directX, hdds);
                        }
                        PCdata = "Processor: " + pcConfig.CPU.Name + '\n' + "Number of cores: " + pcConfig.CPU.Cores + '\n' + "Clock rate: " + pcConfig.CPU.Speed + '\n' +
                            "Video card: " + pcConfig.GPU.Name + '\n' + "VRAM: " + pcConfig.GPU.RAM + '\n';
                        foreach (var item in pcConfig.HDDS)
                        {
                            PCdata += $"Free space on disc {item.Name} " + item.FreeSpace + '\n';
                        }
                        PCdata += "RAM: " + pcConfig.RAM + '\n' +
                        pcConfig.OS.Name + '\n' +
                        "DirectX version: " + pcConfig.DirectX.Name + '\n';
                    }
                    else
                    {
                        throw new Exception("no rows");
                    }
                }
            }
        }
        catch
        {
            PCdata = "a";
            if (PCdata.Length < 3 || toUpdate)
            {
                ManagementObjectSearcher cpu = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                ManagementObjectSearcher gpu = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                ManagementObjectSearcher hdd = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");
                PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");

                using (var connection = new SqliteConnection("Data Source=data.db"))
                {
                    connection.Open();

                    SqliteCommand commandConfig = new SqliteCommand("SELECT * FROM cpu;", connection);

                    //Do something about it
                    //Процессор
                    int cpuId = 0;
                    foreach (ManagementObject c in cpu.Get())
                    {
                        using (SqliteDataReader readerConfig = commandConfig.ExecuteReader())
                        {
                            if (readerConfig.HasRows)
                            {
                                bool cpuIsFound = false;
                                while (readerConfig.Read())
                                {
                                    if (readerConfig.GetString(1) == c["Name"])
                                    {
                                        cpuIsFound = true;
                                        cpuId = readerConfig.GetInt32(0);
                                        break;
                                    }
                                }
                                if (!cpuIsFound)
                                {
                                    SqliteCommand command = new SqliteCommand();
                                    command.Connection = connection;
                                    command.CommandText = $"INSERT INTO CPU(Name, Cores, Speed) VALUES ('{c["Name"]}', '{c["NumberOfCores"]}', '{c["MaxClockSpeed"] + " GHz"}')";
                                    command.ExecuteNonQuery();

                                    SqliteCommand commandGetId = new SqliteCommand($"SELECT id FROM cpu where name = '{c["Name"]}';", connection);
                                    using (SqliteDataReader readerGetId = commandGetId.ExecuteReader())
                                    {
                                        if (readerGetId.HasRows)
                                        {
                                            while (readerGetId.Read())
                                            {
                                                cpuId = readerGetId.GetInt32(0);
                                            }
                                        }
                                    }
                                }
                            }
                            PCdata = "Processor: " + c["Name"] + '\n' + "Number of cores: " + c["NumberOfCores"] + '\n' + "Clock rate: " + c["MaxClockSpeed"] + '\n';
                        }
                    }

                    //Видеокарта
                    commandConfig = new SqliteCommand("SELECT * FROM gpu;", connection);
                    int gpuId = 0;
                    foreach (ManagementObject g in gpu.Get())
                    {
                        using (SqliteDataReader readerConfig = commandConfig.ExecuteReader())
                        {
                            if (readerConfig.HasRows)
                            {
                                bool gpuIsFound = false;
                                while (readerConfig.Read())
                                {
                                    if (readerConfig.GetString(1) == g["Name"])
                                    {
                                        gpuIsFound = true;
                                        gpuId = readerConfig.GetInt32(6);
                                        break;
                                    }
                                }
                                if (!gpuIsFound)
                                {
                                    SqliteCommand command = new SqliteCommand();
                                    command.Connection = connection;
                                    command.CommandText = $"INSERT INTO GPU(Name, RAM) VALUES ('{g["Name"]}', '{Math.Round(Convert.ToInt64(g["AdapterRAM"]) / Math.Pow(2, 30)) + " Gb"}')";
                                    command.ExecuteNonQuery();

                                    SqliteCommand commandGetId = new SqliteCommand($"SELECT id FROM gpu where name = '{g["Name"]}';", connection);
                                    using (SqliteDataReader readerGetId = commandGetId.ExecuteReader())
                                    {
                                        if (readerGetId.HasRows)
                                        {
                                            while (readerGetId.Read())
                                            {
                                                gpuId = readerGetId.GetInt32(0);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        PCdata += "Video card: " + g["Name"] + '\n' + "VRAM: " + Math.Round(Convert.ToInt64(g["AdapterRAM"]) / Math.Pow(2, 30)) + '\n'; //Name, Caption, VideoProcessor
                    }
                    //Оперативка (пока оставим)

                    commandConfig = new SqliteCommand("SELECT * FROM os;", connection);
                    //ОС
                    int osId = 0;
                    foreach (ManagementObject os in searcher.Get())
                    {
                        using (SqliteDataReader readerConfig = commandConfig.ExecuteReader())
                        {
                            if (readerConfig.HasRows)
                            {
                                bool gpuIsFound = false;
                                while (readerConfig.Read())
                                {
                                    if (readerConfig.GetString(1) == RuntimeInformation.OSDescription)
                                    {
                                        gpuIsFound = true;
                                        osId = readerConfig.GetInt32(9);
                                        break;
                                    }
                                }
                                if (!gpuIsFound)
                                {
                                    SqliteCommand command = new SqliteCommand();
                                    command.Connection = connection;
                                    command.CommandText = $"INSERT INTO OS(Name) VALUES ('{RuntimeInformation.OSDescription}')";
                                    command.ExecuteNonQuery();

                                    SqliteCommand commandGetId = new SqliteCommand($"SELECT id FROM os where name = '{RuntimeInformation.OSDescription}';", connection);
                                    using (SqliteDataReader readerGetId = commandGetId.ExecuteReader())
                                    {
                                        if (readerGetId.HasRows)
                                        {
                                            while (readerGetId.Read())
                                            {
                                                osId = readerGetId.GetInt32(0);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    commandConfig = new SqliteCommand("SELECT * FROM directx;", connection);
                    //DirectX
                    int directId = 0;
                    using (SqliteDataReader readerConfig = commandConfig.ExecuteReader())
                    {
                        if (readerConfig.HasRows)
                        {
                            bool gpuIsFound = false;
                            while (readerConfig.Read())
                            {
                                if (readerConfig.GetString(1) == GetDirectXVersion())
                                {
                                    gpuIsFound = true;
                                    directId = readerConfig.GetInt32(4);
                                    break;
                                }
                            }
                            if (!gpuIsFound)
                            {
                                SqliteCommand command = new SqliteCommand();
                                command.Connection = connection;
                                command.CommandText = $"INSERT INTO directx(Name) VALUES ('{GetDirectXVersion()}')";
                                command.ExecuteNonQuery();

                                SqliteCommand commandGetId = new SqliteCommand($"SELECT id FROM os where name = '{GetDirectXVersion()}';", connection);
                                using (SqliteDataReader readerGetId = commandGetId.ExecuteReader())
                                {
                                    if (readerGetId.HasRows)
                                    {
                                        while (readerGetId.Read())
                                        {
                                            directId = readerGetId.GetInt32(0);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    SqliteCommand commandConfig2 = new SqliteCommand();
                    commandConfig2.Connection = connection;

                    // ПОЛНЫЙ ОБЪЕМ ОПЕРАТИВНОЙ ПАМЯТИ
                    ManagementObjectSearcher ram = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");

                    string opka = "";
                    foreach (ManagementObject r in ram.Get())
                    {
                        opka = Math.Round(Convert.ToDouble(r["TotalPhysicalMemory"]) / Math.Pow(2, 30), 2) + " Гб";
                        commandConfig2.CommandText = $"INSERT INTO Config(ńame, gpu, cpu, ram, os, directx) VALUES ('Config{gpuId}', {gpuId}, '{cpuId}', '{opka}', '{osId}', '{directId}')";
                    }

                    int number = commandConfig2.ExecuteNonQuery();

                    foreach (ManagementObject h in hdd.Get())
                    {
                        SqliteCommand command = new SqliteCommand();
                        command.Connection = connection;
                        command.CommandText = $"INSERT INTO HDD(Config, Name, FreeSpace) VALUES ({configId}, '{h["Name"]}', '{Math.Round(Convert.ToInt64(h["FreeSpace"]) / Math.Pow(2, 30)) + " Гб"}')";
                        command.ExecuteNonQuery();
                        PCdata += $"Free space on disc {h["Name"]} " + Math.Round(Convert.ToInt64(h["FreeSpace"]) / Math.Pow(2, 30)) + '\n';
                    }
                    PCdata += "RAM: " + Math.Round(Convert.ToDouble(opka) / Math.Pow(2, 30), 2) + " Гб" + '\n';
                    PCdata += RuntimeInformation.OSDescription + '\n';
                    PCdata += "DirectX version: " + GetDirectXVersion() + '\n';
                    //Console.WriteLine($"В таблицу Users добавлено объектов: {number}");
                    File.WriteAllText("cache/PCdata.txt", PCdata);
                }
            }


            File.WriteAllText("cache/PCdata.txt", PCdata);
        }

        return PCdata;
    }

    public List<CPU> GetCPUs()
    {
        List<CPU> cpus = new List<CPU>();
        using (var connection = new SqliteConnection("Data Source=data.db"))
        {
            connection.Open();

            SqliteCommand command = new SqliteCommand("select * from cpu", connection);
            using (SqliteDataReader readerConfig = command.ExecuteReader())
            {
                if (readerConfig.HasRows)
                {
                    while (readerConfig.Read())
                    {
                        cpus.Add(new CPU(readerConfig.GetInt32(0), readerConfig.GetString(1), readerConfig.GetInt32(2), readerConfig.GetString(3)));
                    }
                }
            }
        }
        return cpus;
    }

    public List<GPU> GetGPUs()
    {
        List<GPU> gpus = new List<GPU>();
        using (var connection = new SqliteConnection("Data Source=data.db"))
        {
            connection.Open();

            SqliteCommand command = new SqliteCommand("select * from gpu", connection);
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        gpus.Add(new GPU(reader.GetInt32(0), reader.GetString(1), reader.GetString(2)));
                    }
                }
            }
        }
        return gpus;
    }

    public List<DirectX> GetDirectXs()
    {
        List<DirectX> directxs = new List<DirectX>();
        using (var connection = new SqliteConnection("Data Source=data.db"))
        {
            connection.Open();

            SqliteCommand command = new SqliteCommand("select * from directx", connection);
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        directxs.Add(new DirectX(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
        }
        return directxs;
    }

    public List<OS> GetOSs()
    {
        List<OS> directxs = new List<OS>();
        using (var connection = new SqliteConnection("Data Source=data.db"))
        {
            connection.Open();

            SqliteCommand command = new SqliteCommand("select * from os", connection);
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        directxs.Add(new OS(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
        }
        return directxs;
    }
    public List<Config> GetConfigs(int userId)
    {
        List<Config> configs = new List<Config>();
        string sqlExpression = $"SELECT \r\n    config.id,\r\n    config.GPU,\r\n    config.CPU,\r\n    config.RAM,\r\n    config.OS,\r\n    config.DirectX,\r\n    cpu.id,\r\n    cpu.Name,\r\n    cpu.Cores,\r\n    cpu.Speed,\r\n    directX.id,\r\n    directX.Name,\r\n    gpu.id,\r\n    gpu.Name,\r\n    gpu.RAM,\r\n    os.id,\r\n    os.Name,\r\n    hdd.id,\r\n    hdd.Config,\r\n    hdd.Name,\r\n    hdd.FreeSpace\r\nFROM config\r\nLEFT JOIN cpu ON config.cpu = cpu.id\r\nLEFT JOIN gpu ON config.gpu = gpu.id\r\nLEFT JOIN directX ON config.directX = directX.id\r\nLEFT JOIN os ON config.os = os.id\r\nLEFT JOIN hdd ON hdd.config = config.id\r\nWHERE config.owner = {userId};";
        using (var connection = new SqliteConnection("Data Source=data.db"))
        {
            connection.Open();

            SqliteCommand command = new SqliteCommand(sqlExpression, connection);
            using (SqliteDataReader readerConfig = command.ExecuteReader())
            {
                if (readerConfig.HasRows)
                {
                    List<HDD> hdds = new List<HDD>();
                    while (readerConfig.Read())
                    {
                        CPU cPU = new CPU(readerConfig.GetInt32(6), readerConfig.GetString(7), readerConfig.GetInt32(8), readerConfig.GetString(9));
                        DirectX directX = new DirectX(readerConfig.GetInt32(10), readerConfig.GetString(11));
                        GPU gPU = new GPU(readerConfig.GetInt32(2), readerConfig.GetString(13), readerConfig.GetString(14));
                        OS oS = new OS(readerConfig.GetInt32(15), readerConfig.GetString(16));
                        hdds.Add(new HDD(readerConfig.GetString(19), readerConfig.GetString(20)));
                        configs.Add(new Config(readerConfig.GetInt32(0), gPU, cPU, readerConfig.GetString(3), oS, directX, hdds));
                    }
                }
                else
                {
                    throw new Exception("no rows");
                }
            }
        }
        return configs;
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
