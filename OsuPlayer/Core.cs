using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using OsuPlayer.Modules.Audio;
using OsuPlayer.Modules.IO;
using OsuPlayer.Views;

namespace OsuPlayer;

public class Core
{
    public static Core Instance { get; protected set; }
    
    public MainWindow MainWindow;
    public BassEngine Engine;
    public Config Config;
    public Player Player; 

    protected internal Core()
    {
        Instance = this;
        Config = Config.LoadConfig();
    }

    protected internal void SetMainWindow(MainWindow window)
    {
        MainWindow = window;
        Engine = BassEngine.Instance;
        Player = new Player();
        Player.ImportSongs();
    }
}