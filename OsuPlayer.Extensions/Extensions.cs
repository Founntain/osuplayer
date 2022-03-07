namespace OsuPlayer.Extensions;

public static class Extensions
{
    public static string FormatTime(this TimeSpan time)
    {
        string timeStr = "";
        if (time.Hours > 0)
        {
            timeStr += time.ToString(@"%h\:mm\:");
        }
        else
        {
            timeStr += time.ToString(@"%m\:");
        }

        timeStr += time.ToString(@"ss");
        return timeStr;
    }
}