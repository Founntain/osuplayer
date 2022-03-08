using OsuPlayer.Audio;
using OsuPlayer.IO;
using OsuPlayer.Views;

namespace OsuPlayer;

public class Core
{
    public static Core Instance { get; protected set; } = new();
    
    public MainWindow MainWindow;
    public Config Config;
    public Player Player; 

    protected internal Core()
    {
        Config = new Config();
    }
}