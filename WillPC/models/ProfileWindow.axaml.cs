using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using HarfBuzzSharp;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
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
        SteamInterations steamInterations = new SteamInterations();
        HardWareInteractons hardWareInteractons = new HardWareInteractons();
        public void ShowPCData()
        {
            List<CPU> cpus = hardWareInteractons.GetCPUs();
            Processors.ItemsSource = cpus;

            List<GPU> gpus = hardWareInteractons.GetGPUs();
            VideoCards.ItemsSource = gpus;

            List<OS> oss = hardWareInteractons.GetOSs();
            OperationSystems.ItemsSource = oss;

            List<DirectX> directXes = hardWareInteractons.GetDirectXs();
            DirectXs.ItemsSource = directXes;

            List<Config> configs = hardWareInteractons.GetConfigs(1);
            Configurations.ItemsSource = configs;

            PCData.Text = hardWareInteractons.GetPCData(0);

            User user = ManagerJSON.DeSerialize<User>("userInfo.json")[0];

            UserUsername.Text = user.Name;
            UserSteamLink.Text = user.SteamLink;
        }
        public void Configurations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Config selectedConfig = Configurations.SelectedItem as Config;

            PCData.Text = hardWareInteractons.GetPCData(
                selectedConfig?.Id ?? 0
            );

        }
        public void CreateNewConfig(object sender, RoutedEventArgs e)
        {
            CPU selectedCPU = Processors.SelectedItem as CPU;
            GPU selectedGPU = VideoCards.SelectedItem as GPU;
            OS selectedOS = OperationSystems.SelectedItem as OS;
            DirectX selectedDirectX = DirectXs.SelectedItem as DirectX;
            string setRAM = ConfigRAM.Text;
            int selectedId = 0;
            List<HDD> selectedHDD = new List<HDD>() { new HDD("Unknown:", ConfigSpace.Text) };
            if (selectedCPU != null && selectedGPU != null &&
                selectedOS != null && selectedDirectX != null &&
                setRAM != null && setRAM != "" &&
                ConfigSpace.Text != null && ConfigSpace.Text != "") 
            {
                using (var connection = new SqliteConnection("Data Source=data.db"))
                {
                    connection.Open();

                    SqliteCommand command = new SqliteCommand("select max(id) from config;", connection);
                    using (SqliteDataReader readerConfig = command.ExecuteReader())
                    {
                        if (readerConfig.HasRows)
                        {
                            while (readerConfig.Read()) { selectedId = readerConfig.GetInt32(0)+1; }
                        }
                    }
                    Config newConfig = new Config(selectedId, $"Config{selectedGPU.Id}", selectedGPU, selectedCPU, setRAM, selectedOS, selectedDirectX, selectedHDD);

                    command = new SqliteCommand();
                    command.Connection = connection;
                    command.CommandText = $"INSERT INTO HDD(Config, Name, FreeSpace) VALUES ({selectedId}, '{selectedHDD[0].Name}', '{selectedHDD[0].FreeSpace}')";
                    command.ExecuteNonQuery();

                    command = new SqliteCommand();
                    command.Connection = connection;
                    command.CommandText = $"INSERT INTO Config(name, gpu, cpu, ram, os, directx, owner) VALUES ('{newConfig.Name}', {newConfig.GPU.Id}, '{newConfig.CPU.Id}', '{newConfig.RAM}', '{newConfig.OS.Id}', '{newConfig.DirectX.Id}', 1)";
                    command.ExecuteNonQuery();

                    Configurations.Items.Add(newConfig);
                }
            }
        }
        public void SavingNewInfo(object sender, RoutedEventArgs e)
        {
            if (UserUsername.Text.Length > 0)
            {
                User aUser = ManagerJSON.DeSerialize<User>("userInfo.json")[0];
                User user = new User(aUser.HasAgreedToUserAgreement, UserUsername.Text, aUser.Password, UserSteamLink.Text);
                ManagerJSON.Serialize<User>(new List<User> { user }, "userInfo.json");
                user.PushToDB();
            }
        }
    }
}