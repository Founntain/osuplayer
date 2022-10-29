namespace OsuPlayer.Extensions;

/// <summary>
/// This attribute defines the default implementation of a service to use if no other is specified explicitly.
/// </summary>
public class DefaultImplAttr : Attribute
{
    /// <summary>
    /// Creates a new instance of the <see cref="DefaultImplAttr" /> class.
    /// </summary>
    public DefaultImplAttr()
    {
    }
}