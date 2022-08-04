using System.Globalization;
using System.Reflection;
using Octokit;

namespace OsuPlayer.Network;

/// <summary>
/// A static class to help us provide updates for the osu!player and check if updates are available by checking our
/// GitHub-Repository releases
/// </summary>
public static class GitHubUpdater
{
    /// <summary>
    /// Checks if the current version is older and needs to be updated
    /// </summary>
    /// <param name="releaseChannel">The release channel to be used</param>
    public static async Task<UpdateResponse> CheckForUpdates(ReleaseChannels releaseChannel = ReleaseChannels.Stable)
    {
        var curVersion = Assembly.GetEntryAssembly()!.GetName().Version;

        var currentVersion = $"{curVersion!.Major}.{curVersion.Minor}.{curVersion.Build}";

        var release = await GetLatestRelease(releaseChannel);

        if (release == default) return new()
        {
            IsNewVersionAvailable = false
        };

        if (currentVersion != release.TagName) return new ()
        {
            IsNewVersionAvailable = true,
            HtmlUrl = release.HtmlUrl,
            IsPrerelease = releaseChannel == ReleaseChannels.PreReleases,
            Version = release.TagName,
            ReleaseDate = release.CreatedAt,
            PatchNotes = await GetLatestPatchNotes(releaseChannel)
        };

        return new UpdateResponse
        {
            IsNewVersionAvailable = false
        };
    }

    /// <summary>
    /// Gets the latest release from GitHub
    /// </summary>
    /// <param name="releaseChannel">The release channel to be used</param>
    /// <returns>a GitHub release</returns>
    public static async Task<Release?> GetLatestRelease(ReleaseChannels releaseChannel = ReleaseChannels.Stable)
    {
        var github = new GitHubClient(new ProductHeaderValue("osu!player"));

        var releases = await github.Repository.Release.GetAll("osu-player", "osuplayer");

        var includePreReleases = releaseChannel == ReleaseChannels.PreReleases;

        Release latestRelease = null;

        foreach (var release in releases.OrderBy(x => x.CreatedAt))
        {
            if(release.Prerelease && !includePreReleases) continue;

            latestRelease = release;
        }

        return latestRelease;
    }

    public static async Task<string> GetLatestPatchNotes(ReleaseChannels releaseChannel = ReleaseChannels.Stable)
    {
        var release = await GetLatestRelease(releaseChannel);

        if (release == default)
            return "**No patch-notes found**";

        return $"## {(release.Prerelease ? "pre-release" : "release")} v" + release.TagName + Environment.NewLine
               + "*released " + release.CreatedAt.ToString("F", new CultureInfo("en-us")) + "*"
               + Environment.NewLine
               + Environment.NewLine
               + release.Body;
    }
}