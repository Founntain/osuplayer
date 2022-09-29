using System.Collections.ObjectModel;
using Avalonia.Media;
using DynamicData;
using OsuPlayer.Extensions.Enums;
using Splat;

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

    public static TService GetRequiredService<TService>(this IReadonlyDependencyResolver resolver)
    {
        var service = resolver.GetService<TService>();

        if (service is null)
            throw new InvalidOperationException($"No service with type of {typeof(TService)}");

        return service;
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

    public static Color ToColor(this KnownColors color) => Color.FromUInt32((uint) color);
    
    public static FontWeights GetNextBiggerFont(this FontWeights font)
    {
        var fontSizes = (FontWeights[]) Enum.GetValues(typeof(FontWeights));    
        
        var i = Array.IndexOf(fontSizes, font) + 1;
        return (fontSizes.Length == i) ? fontSizes[i - 1] : fontSizes[i];
    }
    
    public static FontWeights GetNextSmallerFont(this FontWeights font)
    {
        var fontSizes = (FontWeights[]) Enum.GetValues(typeof(FontWeights));    
        
        var i = Array.IndexOf(fontSizes, font) - 1;
        return (i == -1) ? fontSizes[i + 1] : fontSizes[i];
    }

    public static FontWeight ToFontWeight(this FontWeights font)
    {
        switch (font)
        {
            case FontWeights.Black:
                return FontWeight.Black;
            case FontWeights.Thin:
                return FontWeight.Thin;
            case FontWeights.ExtraLight:
                return FontWeight.ExtraLight;
            case FontWeights.Light:
                return FontWeight.Light;
            case FontWeights.Regular:
                return FontWeight.Regular;
            case FontWeights.Medium:
                return FontWeight.Medium;
            case FontWeights.SemiBold:
                return FontWeight.SemiBold;
            case FontWeights.Bold:
                return FontWeight.Bold;
            case FontWeights.ExtraBold:
                return FontWeight.ExtraBold;
            default:
                return FontWeight.Regular;
        }
    }
}