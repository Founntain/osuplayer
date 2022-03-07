using OsuPlayer.IO;
using OsuPlayer.Views;

namespace OsuPlayer;

public static class Core
{
    public static MainWindow MainWindow;
    public static Config Config;

    public static void Init(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
        Config = Config.LoadConfig();
    }
}