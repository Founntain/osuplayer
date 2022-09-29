using Avalonia.Media;

namespace OsuPlayer.Extensions.Enums;

/// <summary>
/// Defines a set of predefined font weights.
/// </summary>
/// <remarks>
/// As well as the values defined by this enumeration you can also pass any integer value by
/// casting it to <see cref="FontWeight"/>, e.g. <code>(FontWeight)550</code>.
/// </remarks>
public enum FontWeights
{
    /// <summary>
    /// Specifies a "thin" font weight.
    /// </summary>
    Thin = 100,
    
    /// <summary>
    /// Specifies a "extralight" font weight.
    /// </summary>
    ExtraLight = 200,
    
    /// <summary>
    /// Specifies a "light" font weight.
    /// </summary>
    Light = 300,

    /// <summary>
    /// Specifies a "regular" font weight.
    /// </summary>
    Regular = 400,

    /// <summary>
    /// Specifies a "medium" font weight.
    /// </summary>
    Medium = 500,

    /// <summary>
    /// Specifies a "semibold" font weight.
    /// </summary>
    SemiBold = 600,
    
    /// <summary>
    /// Specifies a "bold" font weight.
    /// </summary>
    Bold = 700,

    /// <summary>
    /// Specifies an "extra bold" font weight.
    /// </summary>
    ExtraBold = 800,
    
    /// <summary>
    /// Specifies an "black" font weight.
    /// </summary>
    Black = 900,
}