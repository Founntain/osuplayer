using OsuPlayer.IO.Storage.LazerModels.Interfaces;

namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

public interface IScoreInfo : IHasOnlineID<long>, IHasNamedFiles
{
    IUser User { get; }

    long TotalScore { get; }

    int MaxCombo { get; }

    double Accuracy { get; }

    bool HasReplay { get; }

    DateTimeOffset Date { get; }

    double? PP { get; }

    IBeatmapInfo Beatmap { get; }

    IRulesetInfo Ruleset { get; }

    ScoreRank Rank { get; }

    // Mods is currently missing from this interface as the `IMod` class has properties which can't be fulfilled by `APIMod`,
    // but also doesn't expose `Settings`. We can consider how to implement this in the future if required.

    // Statistics is also missing. This can be reconsidered once changes in serialisation have been completed.
}