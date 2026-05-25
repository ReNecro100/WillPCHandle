using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

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
<<<<<<< HEAD
                desktop.MainWindow = new MainWindowTest();//GamePageWindow
=======
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
>>>>>>> GUI
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
