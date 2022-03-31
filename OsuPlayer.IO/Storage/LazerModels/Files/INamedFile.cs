namespace OsuPlayer.IO.Storage.LazerModels.Files;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
public interface INamedFile
{
    string Filename { get; set; }

    RealmFile File { get; set; }
}