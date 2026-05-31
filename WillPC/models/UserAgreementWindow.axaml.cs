using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.ComponentModel;
namespace WillPC
{
    public partial class UserAgreementWindow : Window
    {
        private bool _canClose;
        public bool Accepted { get; private set; }

        public UserAgreementWindow()
        {
            
            InitializeComponent();
            Closing += UserAgreementWindow_Closing;
            
        }
        private void UserAgreementWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (!_canClose)
            {
                e.Cancel = true;
            }
        }
        private void AcceptAgreement(object sender, RoutedEventArgs e)
        {
            Accepted = true;
            List<User> aUser = ManagerJSON.DeSerialize<User>("userInfo.json");
            User user = new User(Accepted, aUser[0].Name, aUser[0].Password, aUser[0].SteamLink);
            ManagerJSON.Serialize<User>(new List<User>{ user }, "userInfo.json");
            _canClose = true;
            Close();
        }

        private void DeclineAccept(object sender, RoutedEventArgs e)
        {
            _canClose = true;
            if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }
    }
    
}

