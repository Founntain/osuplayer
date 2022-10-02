using Avalonia.Controls;
using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Data.OsuPlayer.StorageModels;

public class ConfigContainer : IStorableContainer
{
    public string? OsuPath { get; set; }
    public double Volume { get; set; }
    public bool UseSongNameUnicode { get; set; } = false;
    public int SelectedAudioDevice { get; set; } = 0;
    public bool IsEqEnabled { get; set; } = false;
    public WindowTransparencyLevel TransparencyLevelHint { get; set; } = WindowTransparencyLevel.AcrylicBlur;
    public StartupSong StartupSong { get; set; } = StartupSong.FirstSong;
    public SortingMode SortingMode { get; set; } = SortingMode.Title;
    public RepeatMode RepeatMode { get; set; } = RepeatMode.NoRepeat;
    public Guid? ActivePlaylistId { get; set; }
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

    public IStorableContainer Init()
    {
        return this;
    }

    public FontWeights GetSmallerFont()
    {
        return DefaultFontWeight.GetNextSmallerFont();
    }

    public FontWeights GetBiggerFont()
    {
        return DefaultFontWeight.GetNextBiggerFont();
    }
}