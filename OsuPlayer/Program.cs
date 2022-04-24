using System;
using Avalonia;
using Avalonia.ReactiveUI;
using OsuPlayer.Extensions;
using OsuPlayer.Modules.Audio;
using OsuPlayer.Windows;
using Splat;

namespace OsuPlayer;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            Register(Locator.CurrentMutable, Locator.Current);
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            UnhandledExceptionHandler.HandleException(ex);
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseSkia()
            .UseReactiveUI()
            .With(new Win32PlatformOptions
            {
                AllowEglInitialization = true
            });
    }

    private static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton(() => new BassEngine());

        services.RegisterLazySingleton(() => new Player(
            resolver.GetService<BassEngine>()));

        services.Register(() => new MainWindowViewModel(
            resolver.GetService<BassEngine>(),
            resolver.GetService<Player>()));

        services.RegisterLazySingleton(() => new MainWindow(
            resolver.GetService<MainWindowViewModel>(),
            resolver.GetService<Player>()));
    }
}