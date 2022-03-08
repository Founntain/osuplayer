using System;

namespace OsuPlayer.Modules.Extensions;

public static class Extensions
{
    public static string FormatTime(this TimeSpan time)
    {
        var timeStr = string.Empty;

        timeStr += time.Hours > 0
            ? time.ToString(@"%h\:mm\:")
            : time.ToString(@"%m\:");

        timeStr += time.ToString(@"ss");

        return timeStr;
    }
}