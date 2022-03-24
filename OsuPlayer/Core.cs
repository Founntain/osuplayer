using OsuPlayer.IO.Storage.Config;
using OsuPlayer.Modules.Audio;
using OsuPlayer.Windows;

namespace OsuPlayer;
public class Core
{
    public BassEngine Engine;

    public MainWindow MainWindow;
    public Player Player;

    protected internal Core()
    {
        Instance = this;
        Player = new Player();
    }

    public static Core Instance { get; private set; }

    protected internal async void SetupCore(MainWindow window)
    {
        MainWindow = window;
        Engine = BassEngine.Instance;
        
        await using (var config = new Config())
        {
            await config.ReadAsync();
            MainWindow.TransparencyLevelHint = config.Container.TransparencyLevelHint;
        }

        await Player.ImportSongs();
    }
}