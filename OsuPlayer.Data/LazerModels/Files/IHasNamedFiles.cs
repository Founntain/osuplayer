namespace OsuPlayer.Data.LazerModels.Files;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
public interface IHasNamedFiles
{
    /// <summary>
    /// All files used by this model.
    /// </summary>
    IEnumerable<INamedFileUsage> Files { get; }
}