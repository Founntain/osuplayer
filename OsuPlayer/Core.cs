using OsuPlayer.Modules.Audio;
using OsuPlayer.Modules.IO;
using OsuPlayer.Views;
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
    }

    public static Core Instance { get; protected set; }

    protected internal void SetMainWindow(MainWindow window)
    {
        MainWindow = window;
        MainWindow.TransparencyLevelHint = Config.TransparencyLevelHint;
        
        Engine = BassEngine.Instance;
        
        Player = new Player();
        Player.ImportSongs();
    }
}