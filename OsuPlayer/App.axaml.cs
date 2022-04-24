using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using OsuPlayer.Modules;
using OsuPlayer.Windows;
using ReactiveUI;
using Splat;

namespace OsuPlayer;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) desktop.MainWindow = Locator.Current.GetService<MainWindow>();

        base.OnFrameworkInitializationCompleted();
    }
}