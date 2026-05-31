using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;

namespace WillPC
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                //Nakatitj logiku z JSON
                List<User> aUser = ManagerJSON.DeSerialize<User>("userInfo.json");

                bool isAgreed = aUser[0].HasAgreedToUserAgreement;
                if (isAgreed)
                {
                    desktop.MainWindow = new MainWindow();
                }
                else
                {
                    var agreementWindow = new UserAgreementWindow();

                    agreementWindow.Closed += (_, _) =>
                    {
                        if (!agreementWindow.Accepted)
                        {
                            return;
                        }

                        var mainWindow = new MainWindow();
                        desktop.MainWindow = mainWindow;
                        mainWindow.Show();
                    };

                    desktop.MainWindow = agreementWindow;
                }
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}