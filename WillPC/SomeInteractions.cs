using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;


public static class ManagerJSON
{
    public static List<T>? DeSerialize<T>(string path)
    {
        try
        {
            string jsonString = File.ReadAllText(path);
            List<T>? supplies = JsonSerializer.Deserialize<List<T>>(jsonString);
            return supplies;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
    public static int Serialize<T>(List<T> supplies, string filename)
    {
        try
        {
            if (supplies is null || supplies.Count == 0) throw new ArgumentNullException(nameof(supplies));
            string s = JsonSerializer.Serialize(supplies);
            File.WriteAllText(filename, s);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return 1;
        }
    }
}

