using System.Collections.ObjectModel;
using DynamicData;
using Splat;

namespace OsuPlayer.Extensions;

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

    public static ObservableCollection<T> ToObservableCollection<T>(this ICollection<T>? source)
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

        T[] arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(arr, src) + 1;
        return (arr.Length==j) ? arr[0] : arr[j];            
    }
}