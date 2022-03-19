using OsuPlayer.Extensions;
using OsuPlayer.IO;
using OsuPlayer.IO.Storage;
using OsuPlayer.Modules.Audio;
using MainWindow = OsuPlayer.Windows.MainWindow;

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
        using (var config = new Config())
        {
            await config.ReadAsync();
            MainWindow.TransparencyLevelHint = config.Container.TransparencyLevelHint;
        }
        
        Engine = BassEngine.Instance;

        await Player.ImportSongs();
    }
}