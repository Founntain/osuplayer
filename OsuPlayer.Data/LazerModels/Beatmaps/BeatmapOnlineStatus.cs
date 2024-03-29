namespace OsuPlayer.Data.LazerModels.Beatmaps;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
public enum BeatmapOnlineStatus
{
    None = -3,

    Graveyard = -2,

    WIP = -1,

    Pending = 0,

    Ranked = 1,

    Approved = 2,

    Qualified = 3,

    Loved = 4
}