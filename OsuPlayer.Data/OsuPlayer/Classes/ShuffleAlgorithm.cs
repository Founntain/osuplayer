using System.Reflection;
using System.Text.RegularExpressions;

namespace OsuPlayer.Data.OsuPlayer.Classes;

/// <summary>
/// A class that contains information about a shuffle algorithm.
/// </summary>
public class ShuffleAlgorithm
{
    public Type Type { get; }
    public string Name { get; }
    public string Description { get; }

    public override string ToString()
    {
        return Name;
    }

    public ShuffleAlgorithm(Type type)
    {
        Type = type;
        var info = type.GetCustomAttribute<ImplInfoAttr>();
        Name = info?.Name ?? Regex.Replace(type.Name, "([A-Z])", " $1").Trim();
        Description = info?.Description ?? "";
    }
}