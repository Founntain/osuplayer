using Avalonia.Controls;
using Avalonia.Media;

namespace OsuPlayer.Data.OsuPlayer.Enums;

public enum BackgroundMode
{
    SolidColor = WindowTransparencyLevel.None,
    AcrylicBlur = WindowTransparencyLevel.AcrylicBlur,
    Mica = WindowTransparencyLevel.Mica
}