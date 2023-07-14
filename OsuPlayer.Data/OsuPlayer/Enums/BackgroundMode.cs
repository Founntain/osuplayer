using Avalonia.Controls;

namespace OsuPlayer.Data.OsuPlayer.Enums;

public enum BackgroundMode
{
    SolidColor = 0,
    AcrylicBlur = 1,
    Mica = 2
}

public static class BackgroundModeExtensions
{
    private static readonly List<WindowTransparencyLevel> WindowTransparencyLevels = new();

    public static IReadOnlyList<WindowTransparencyLevel> ToWindowTransparencyLevelList(this BackgroundMode backgroundMode)
    {
        WindowTransparencyLevels.Clear();

        switch (backgroundMode)
        {
            case BackgroundMode.AcrylicBlur:
                WindowTransparencyLevels.Add(WindowTransparencyLevel.AcrylicBlur);

                break;
            case BackgroundMode.Mica:
                WindowTransparencyLevels.Add(WindowTransparencyLevel.Mica);

                break;
            case BackgroundMode.SolidColor:
            default:
                WindowTransparencyLevels.Add(WindowTransparencyLevel.None);

                break;
        }

        return WindowTransparencyLevels;
    }
}