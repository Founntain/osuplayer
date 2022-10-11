using System.Text.RegularExpressions;

namespace OsuPlayer.Data.OsuPlayer.Classes;

/// <summary>
/// A class that contains information about a shuffle algorithm.
/// </summary>
public class ShuffleAlgorithm
{
    public Type Type { get; }
    private readonly string _formattedName;

    public override string ToString()
    {
        return _formattedName;
    }

    public ShuffleAlgorithm(Type type)
    {
        Type = type;
        _formattedName = Regex.Replace(type.Name, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
    }
}