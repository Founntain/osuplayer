using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using DynamicData;

namespace OsuPlayer.Extensions;

/// <summary>
/// A static class to contain extension methods for various types
/// </summary>
public static class Extensions
{
    public static string FormatTime(this TimeSpan time)
    {
        var timeStr = string.Empty;

        timeStr += time.Hours > 0
            ? time.ToString(@"%h\:mm\:")
            : time.ToString(@"%m\:");

        timeStr += time.ToString(@"ss");

        return timeStr;
    }

    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T>? source)
    {
        if (source == default)
            return new ObservableCollection<T>();

        return new ObservableCollection<T>(source);
    }

    public static SourceList<T> ToSourceList<T>(this IEnumerable<T>? source)
    {
        if (source == default)
            return new SourceList<T>();

        var sl = new SourceList<T>();

        sl.AddRange(source);

        return sl;
    }

    public static bool IsDigitsOnly(this string str)
    {
        foreach (var c in str)
            if (c < '0' || c > '9')
                return false;

        return true;
    }

    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

        var arr = (T[]) Enum.GetValues(src.GetType());
        var j = Array.IndexOf(arr, src) + 1;
        return arr.Length == j ? arr[0] : arr[j];
    }

    public static bool IsInBounds<T>(this IEnumerable<T> enumerable, int index) where T : class
    {
        return index >= 0 && index < enumerable.Count();
    }

    public static float GetPerceivedBrightness(this Color color)
    {
        return (color.R * 0.299f + color.G * 0.587f + color.B * 0.114f) / 256f;
    }

    public static string ToVersionString(this Assembly? assembly)
    {
        var version = assembly?.GetName().Version;

        if (version == null) return string.Empty;

        return $"{version.Major}.{version.Minor}.{version.Build}";
    }
}