using System.IO;
using Newtonsoft.Json;

namespace OsuPlayer.Modules.IO;

public class Config
{
    public string? OsuPath { get; set; }
    public string OsuSongsPath => $"{OsuPath}\\Songs";

    public bool UseSongnameUnicode { get; set; } = false;

    public int SelectedOutputDevice { get; set; } = 0;
    public bool IsEqEnabled { get; set; } = false;

    /// <summary>
    /// Load config, if none was found create a new one
    /// </summary>
    /// <returns>Returns a <see cref="Config"/> object</returns>
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
        return LoadConfig();
    }

    public void SaveConfig()
    {
        File.WriteAllText("data/config.json", JsonConvert.SerializeObject(this));
    }
}