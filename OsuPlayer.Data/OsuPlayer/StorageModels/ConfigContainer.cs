using Avalonia.Controls;
using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Data.OsuPlayer.StorageModels;

public class ConfigContainer : IStorableContainer
{
    public string? OsuPath { get; set; }
    public double Volume { get; set; }
    public bool UseSongNameUnicode { get; set; } = false;
    public string? SelectedAudioDeviceDriver { get; set; }
    public bool IsEqEnabled { get; set; }
    public StartupSong StartupSong { get; set; } = StartupSong.FirstSong;
    public SortingMode SortingMode { get; set; } = SortingMode.Title;
    public RepeatMode RepeatMode { get; set; } = RepeatMode.NoRepeat;
    public Guid? SelectedPlaylist { get; set; }
    public string? ShuffleAlgorithm { get; set; } = "RngShuffler";
    public bool IsShuffle { get; set; }
    public string? LastPlayedSong { get; set; }
    public bool IgnoreSongsWithSameNameCheckBox { get; set; }
    public bool BlacklistSkip { get; set; }
    public bool PlaylistEnableOnPlay { get; set; }
    public string? Username { get; set; }
    public ReleaseChannels ReleaseChannel { get; set; } = 0;
    public KnownColors BackgroundColor { get; set; } = KnownColors.Black;
    public KnownColors AccentColor { get; set; } = KnownColors.White;
    public FontWeights DefaultFontWeight { get; set; } = FontWeights.Medium;
    public string? Font { get; set; }
    public bool UseDiscordRpc { get; set; }
    public bool UsePitch { get; set; } = true;
    public BackgroundMode BackgroundMode { get; set; } = BackgroundMode.AcrylicBlur;
    public float BackgroundBlurRadius { get; set; } = 50f;
    public bool DisplayBackgroundImage { get; set; } = false;
    public string LastFmApiKey { get; set; }
    public string LastFmSecret { get; set; }
    public bool EnableScrobbling { get; set; } = false;

    public IStorableContainer Init()
    {
        return this;
    }
}