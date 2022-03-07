using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using OsuPlayer.ViewModels;
using OsuPlayer.Views;

namespace OsuPlayer
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    ViewModel = new MainWindowBaseViewModel()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}