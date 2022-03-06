using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Data.OsuPlayer.Classes
{
    /// <summary>
    ///     Contains the values from the config.json
    /// </summary>
    public sealed class ConfigStorage
    {
        public int BlurStrength { get; set; }

        public float Volume { get; set; }

        //public double[] EQSettings { get; set; } = new double[10];
        public bool IsEqEnabled { get; set; }
        public StartupSong StartupSong { get; set; }
        public bool LoadCustomSongs { get; set; }
        public string OsuPath { get; set; }
        public string OsuSongsPath { get; set; }
        public string OsuPlayerPlusDirectory { get; set; }
        public bool UseObs { get; set; }
        public double ParalaxStrength { get; set; }
        public bool AutoDownload { get; set; }
        public string Language { get; set; }
        public bool UseDiscord { get; set; }
        public bool CacheSongs { get; set; }
        public int SelectedOutputDevice { get; set; }
        public bool OfflineMode { get; set; }
        public bool Shuffle { get; set; }
        public bool IgnoreSongsWithSameNameCheckBox { get; set; }
        public bool StartMiniPlayerOnStartup { get; set; }
        public int BackgroundDim { get; set; } = 100;
    }
}