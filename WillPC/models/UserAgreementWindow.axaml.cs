using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using System.ComponentModel;
namespace WillPC
{
    public partial class UserAgreementWindow : Window
    {
        private bool _canClose;
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

