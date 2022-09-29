using Avalonia.Controls;
using Avalonia.Media;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;
using OsuPlayer.Extensions.Enums;
using OsuPlayer.Network;

namespace OsuPlayer.IO.Storage.Config;

public class ConfigContainer : IStorableContainer
{
    public string? OsuPath { get; set; }
    public double Volume { get; set; }
    public bool UseSongNameUnicode { get; set; } = false;
    public int SelectedOutputDevice { get; set; }
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
    public FontWeights DefaultFontWeight { get; set; } = FontWeights.Medium;

    public IStorableContainer Init()
    {
        return this;
    }

    public FontWeights? GetSmallFont() => DefaultFontWeight.GetNextSmallerFont();
    
    public FontWeights? GetBiggerFont() => DefaultFontWeight.GetNextBiggerFont();
}