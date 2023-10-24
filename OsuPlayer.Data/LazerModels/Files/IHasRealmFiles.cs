namespace OsuPlayer.Data.LazerModels.Files;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
public interface IHasRealmFiles
{
    IList<RealmNamedFileUsage> Files { get; }

    string Hash { get; set; }
}