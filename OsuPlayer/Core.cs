using OsuPlayer.IO;
using OsuPlayer.Modules.Audio;
using MainWindow = OsuPlayer.Windows.MainWindow;

namespace OsuPlayer;

public class Core
{
    public Config Config;
    public BassEngine Engine;
    
    public MainWindow MainWindow;
    public Player Player;

    protected internal Core()
    {
        Instance = this;
        Config = Config.LoadConfig();
        Config.Instance = Config;
        Player = new Player();
    }

    public static Core Instance { get; private set; }

    protected internal async void SetupCore(MainWindow window)
    {
        MainWindow = window;
        MainWindow.TransparencyLevelHint = Config.TransparencyLevelHint;
        
        Engine = BassEngine.Instance;

        await Player.ImportSongs();
    }
}