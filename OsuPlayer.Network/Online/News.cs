using System.Globalization;

namespace OsuPlayer.Network.Online;

public sealed class News
{
    public DateTime CreationTime { get; set; }

    public string CreationTimeString =>
        $"Written on {CreationTime.ToString("dddd HH:mm, dd MMMM yyyy", new CultureInfo("en-us"))} by {Creator}";

    public string Creator { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}