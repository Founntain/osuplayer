using System.Diagnostics;
using Avalonia;
using Avalonia.ReactiveUI;
using OsuPlayer.Extensions;
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
        catch (Exception ex) //If we have an unhandled exception we catch it here
        {
#if DEBUG
            // If we debug the application and an unhandled exception is thrown,
            // we need to initiate a break for the debugger, so we can debug the exception.
            // Because we handle it above, the application just closes and logs it.
            // This avoids opening the logs and we can debug it directly.
            Debugger.Break();
#endif
            
            //Create crashlog for users
            UnhandledExceptionHandler.HandleException(ex);
      
            var processStartInfo = new ProcessStartInfo("dotnet", "OsuPlayer.CrashHandler.dll");

            processStartInfo.CreateNoWindow = true;
            
            Process.Start(processStartInfo);
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