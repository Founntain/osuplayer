using System.ComponentModel;

namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

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
    XH,
}