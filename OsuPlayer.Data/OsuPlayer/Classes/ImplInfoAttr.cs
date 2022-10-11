namespace OsuPlayer.Data.OsuPlayer.Classes;

/// <summary>
/// This attribute provides some information about a specific interface implementation to be accessed at runtime.
/// </summary>
public class ImplInfoAttr : Attribute
{
    public string Name { get; }
    public string Description { get; }

    /// <summary>
    /// Creates a new instance of <see cref="ImplInfoAttr"/>.
    /// </summary>
    /// <param name="name">The implementation name</param>
    /// <param name="description">The implementation description</param>
    public ImplInfoAttr(string name, string description)
    {
        Name = name;
        Description = description;
    }
}