using System.Diagnostics;
using Avalonia;
using Avalonia.ReactiveUI;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Importer;
using OsuPlayer.Modules.Audio.Engine;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Modules.Services;
using OsuPlayer.Windows;
using Splat;

namespace OsuPlayer;

internal static class Program
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
        catch (Exception ex) //If we have an unhandled exception we catch it here
        {
#if DEBUG
            // If we debug the application and an unhandled exception is thrown,
            // we need to initiate a break for the debugger, so we can debug the exception.
            // Because we handle it above, the application just closes and logs it.
            // This avoids opening the logs and we can debug it directly.
            Debugger.Break();
#endif

            // Create crashlog for users
            UnhandledExceptionHandler.HandleException(ex);

            // Start the CrashHandler to display the error message to the user
            var processStartInfo = new ProcessStartInfo("dotnet", "OsuPlayer.CrashHandler.dll")
            {
                CreateNoWindow = true
            };

            Process.Start(processStartInfo);
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseSkia()
            .UseReactiveUI()
            .With(new Win32PlatformOptions
            {
                AllowEglInitialization = true,
                UseWindowsUIComposition = true
            });
    }

    private static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton<IAudioEngine>(() => new BassEngine());

        services.RegisterLazySingleton<IShuffleProvider>(() => new AltSongShuffler());
        services.RegisterLazySingleton<IStatisticsProvider>(() => new ApiStatisticsProvider());
        services.RegisterLazySingleton<ISortProvider>(() => new SortProvider());
        services.RegisterLazySingleton<ISongSourceProvider>(() => new OsuSongSourceProvider(resolver.GetService<ISortProvider>()));

        services.RegisterLazySingleton<IPlayer>(() => new Player(
            resolver.GetRequiredService<IAudioEngine>(),
            resolver.GetRequiredService<ISongSourceProvider>(),
            resolver.GetService<IShuffleProvider>(),
            resolver.GetService<IStatisticsProvider>(),
            resolver.GetService<ISortProvider>()));

        services.Register(() => new MainWindowViewModel(
            resolver.GetRequiredService<IAudioEngine>(),
            resolver.GetRequiredService<IPlayer>(),
            resolver.GetService<IStatisticsProvider>(),
            resolver.GetService<ISortProvider>()));

        services.RegisterLazySingleton(() => new MainWindow(
            resolver.GetRequiredService<MainWindowViewModel>()));
    }
}