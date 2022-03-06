using Avalonia.Media;

namespace OsuPlayerPlus.Classes.Online;

public static class UserColors
{
    public static Brush Developer => new SolidColorBrush(Color.FromRgb(255, 0, 152));
    public static Brush Tester => (Brush) Brushes.DeepSkyBlue;
    public static Brush Translator => (Brush) Brushes.Chartreuse;
    public static Brush LegacyUser => new SolidColorBrush(Color.FromRgb(255, 194, 93));
    public static Brush Donator => (Brush) Brushes.Gold;
    public static Brush User => (Brush) Brushes.Moccasin;
}