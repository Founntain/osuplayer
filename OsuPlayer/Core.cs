using OsuPlayer.Audio;
using OsuPlayer.IO;
using OsuPlayer.Views;

namespace OsuPlayer;

public class Core
{
    public static Core Instance { get; protected set; }
    
    public MainWindow MainWindow;
    public Config Config;
    public Player Player; 

    protected internal Core(MainWindow window)
    {
        Instance = this;
        MainWindow = window;
        Config = Config.LoadConfig();
        Player = new Player();
        Player.ImportSongs();
    }
}