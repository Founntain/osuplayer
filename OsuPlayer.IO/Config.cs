using Avalonia.Controls;
using Newtonsoft.Json;

namespace OsuPlayer.IO;

public class Config
{
    public static Config? Instance { get; set; }
    
    public string? OsuPath { get; set; }
    public string OsuSongsPath => $"{OsuPath}\\Songs";

    public double Volume { get; set; }

    public bool UseSongNameUnicode { get; set; } = false;

    public int SelectedOutputDevice { get; set; }
    public bool IsEqEnabled { get; set; } = false;
    public WindowTransparencyLevel TransparencyLevelHint { get; set; } = WindowTransparencyLevel.AcrylicBlur;

    /// <summary>
    ///     Load config, if none was found create a new one
    /// </summary>
    /// <returns>Returns a <see cref="Config" /> object</returns>
    public static Config LoadConfig()
    {
        DirectoryManager.GenerateMissingDirectories();

        if (File.Exists("data/config.json"))
        {
            var data = File.ReadAllText("data/config.json");

            return (string.IsNullOrWhiteSpace(data)
                ? new Config()
                : JsonConvert.DeserializeObject<Config>(data))!;
        }

        File.WriteAllText("data/config.json", JsonConvert.SerializeObject(new Config()));

        return new Config();
    }

    public static Config GetConfigInstance()
    {
        return Instance ?? LoadConfig();
    }

    public void SaveConfig()
    {
        File.WriteAllText("data/config.json", JsonConvert.SerializeObject(this));
    }
}