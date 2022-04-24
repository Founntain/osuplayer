using Avalonia.Controls;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.Enums;

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
    public LastPlayedSongModel? LastPlayedSong { get; set; }
    public bool IgnoreSongsWithSameNameCheckBox { get; set; }
}