using Avalonia.Media;

namespace OsuPlayer.Network.Online;

/// <summary>
/// A list of all available role colors
/// </summary>
public static class UserColors
{
    public static Brush Developer => new SolidColorBrush(Color.FromRgb(255, 0, 152));
    public static Brush Tester => new SolidColorBrush(Color.FromRgb(0, 191, 255));
    public static Brush Translator => new SolidColorBrush(Color.FromRgb(127, 255, 0));
    public static Brush LegacyUser => new SolidColorBrush(Color.FromRgb(255, 194, 93));
    public static Brush Donator => new SolidColorBrush(Color.FromRgb(255, 215, 0));
    public static Brush User => new SolidColorBrush(Color.FromRgb(255, 255, 255));
}