using System.ComponentModel;

namespace OsuPlayer.Data.LazerModels.Beatmaps;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
public enum ScoreRank
{
    [Description(@"D")]
    D,

    [Description(@"C")]
    C,

    [Description(@"B")]
    B,

    [Description(@"A")]
    A,

    [Description(@"S")]
    S,

    [Description(@"S+")]
    SH,

    [Description(@"SS")]
    X,

    [Description(@"SS+")]
    XH
}