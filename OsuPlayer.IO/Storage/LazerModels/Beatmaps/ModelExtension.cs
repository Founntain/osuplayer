using OsuPlayer.IO.Storage.LazerModels.Interfaces;

namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

public static class ModelExtension
{
    /// <summary>
    /// Get the relative path in osu! storage for this file.
    /// </summary>
    /// <param name="fileInfo">The file info.</param>
    /// <returns>A relative file path.</returns>
    public static string GetStoragePath(this IFileInfo fileInfo) => Path.Combine(fileInfo.Hash.Remove(1), fileInfo.Hash.Remove(2), fileInfo.Hash);

    /// <summary>
    /// Returns a user-facing string representing the <paramref name="model"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Non-interface types without special handling will fall back to <see cref="object.ToString()"/>.
    /// </para>
    /// <para>
    /// Warning: This method is _purposefully_ not called <c>GetDisplayTitle()</c> like the others, because otherwise
    /// extension method type inference rules cause this method to call itself and cause a stack overflow.
    /// </para>
    /// </remarks>
    public static string GetDisplayString(this object? model)
    {
        string? result = null;

        switch (model)
        {
            case IBeatmapSetInfo beatmapSetInfo:
                result = beatmapSetInfo.Metadata.GetDisplayTitle();
                break;

            case IBeatmapInfo beatmapInfo:
                result = beatmapInfo.GetDisplayTitle();
                break;

            case IBeatmapMetadataInfo metadataInfo:
                result = metadataInfo.GetDisplayTitle();
                break;

            case IScoreInfo scoreInfo:
                result = scoreInfo.GetDisplayTitle();
                break;

            case IRulesetInfo rulesetInfo:
                result = rulesetInfo.Name;
                break;

            case IUser user:
                result = user.Username;
                break;
        }

        // fallback in case none of the above happens to match.
        result ??= model?.ToString() ?? @"null";
        return result;
    }
}