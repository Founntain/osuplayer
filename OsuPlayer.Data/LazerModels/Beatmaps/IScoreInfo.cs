using OsuPlayer.Data.LazerModels.Files;
using OsuPlayer.Data.LazerModels.Interfaces;

namespace OsuPlayer.Data.LazerModels.Beatmaps;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
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