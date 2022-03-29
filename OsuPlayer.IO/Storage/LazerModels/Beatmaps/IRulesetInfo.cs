using OsuPlayer.IO.Storage.LazerModels.Interfaces;

namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

public interface IRulesetInfo : IHasOnlineID<int>, IEquatable<IRulesetInfo>, IComparable<IRulesetInfo>
{
    /// <summary>
    /// The user-exposed name of this ruleset.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// An acronym defined by the ruleset that can be used as a permanent identifier.
    /// </summary>
    string ShortName { get; }

    /// <summary>
    /// A string representation of this ruleset, to be used with reflection to instantiate the ruleset represented by this
    /// metadata.
    /// </summary>
    string InstantiationInfo { get; }
}