using OsuPlayer.Data.DataModels.Interfaces;

namespace OsuPlayer.Data.DataModels;

/// <summary>
/// a full beatmap entry with optionally used data
/// <remarks>only created on a <see cref="IMapEntryBase.ReadFullEntry" /> call</remarks>
/// </summary>
public class RealmMapEntry : RealmMapEntryBase, IMapEntry
{
    public string BackgroundFileLocation { get; init; } = string.Empty;
    public string AudioFileName { get; init; } = string.Empty;
    public string FolderName { get; init; } = string.Empty;
    public string FolderPath { get; init; } = string.Empty;
    public string FullPath { get; init; } = string.Empty;

    public Task<string?> FindBackground()
    {
        return Task.FromResult(File.Exists(BackgroundFileLocation) ? BackgroundFileLocation : null);
    }
}