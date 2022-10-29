using JetBrains.Annotations;
using Realms;

namespace OsuPlayer.IO.Storage.LazerModels.Files;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
public class RealmNamedFileUsage : EmbeddedObject, INamedFile, INamedFileUsage
{
    public RealmFile File { get; set; } = null!;

    // [Indexed] cannot be used on `EmbeddedObject`s as it only applies to top-level queries. May need to reconsider this if performance becomes a concern.
    public string Filename { get; set; } = null!;

    IFileInfo INamedFileUsage.File => File;

    public RealmNamedFileUsage(RealmFile file, string filename)
    {
        File = file;
        Filename = filename;
    }

    [UsedImplicitly]
    private RealmNamedFileUsage()
    {
    }
}