using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Tmds.DBus.Protocol;


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

public class User
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string SteamLink { get; set; }
    public User() { }
    public User(string name, string password, string steamLink)
    {
        Name = name;
        Password = password;
        SteamLink = steamLink;
    }
    public void PushToDB()
    {
        using (var connection = new SqliteConnection("Data Source=data.db"))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand();
            command.Connection = connection;
            command.CommandText = $"INSERT INTO User(Name, Password, SteamLink) VALUES ('{Name}', '{Password}', '{SteamLink}')";
            command.ExecuteNonQuery();
        }
    }
    public List<String> PullFromDB(int userId) 
    {
        string sqlExpression = $"select * from user where id = {userId}";
        List<String> list = new List<String>();
        using (var connection = new SqliteConnection("Data Source=data.db"))
        {
            connection.Open();

            SqliteCommand command = new SqliteCommand(sqlExpression, connection);
            using (SqliteDataReader readerConfig = command.ExecuteReader())
            {
                if (readerConfig.HasRows)
                {
                    while (readerConfig.Read())
                    {
                        list.Add(readerConfig.GetString(1));
                        list.Add(readerConfig.GetString(2));
                        list.Add(readerConfig.GetString(3));
                    }
                }
                else
                {
                    throw new Exception("no rows");
                }
            }
        }
        return list;
    }
    public void UpdateConfig(int userId, int newConfig)
    {
        using (var connection = new SqliteConnection("Data Source=data.db"))
        {
            connection.Open();
            SqliteCommand command = new SqliteCommand();
            command.Connection = connection;
            command.CommandText = $"update user set config = {newConfig} where id = {userId}";
            command.ExecuteNonQuery();
        }
    }
}
