﻿using System.Text;
using Nein.Extensions;
using OsuPlayer.IO.Storage.LazerModels.Beatmaps;
using OsuPlayer.IO.Storage.LazerModels.Files;
using Realms;
using Realms.Dynamic;

namespace OsuPlayer.IO.DbReader.DataModels;

/// <summary>
/// a minimal beatmap entry with only frequently used data
/// <remarks>created on call of <see cref="RealmReader.Read(string)" /></remarks>
/// </summary>
public class RealmMapEntryBase : IMapEntryBase
{
    public Guid Id { get; init; }
    public string? OsuPath { get; init; }
    public string Artist { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Hash { get; init; } = string.Empty;
    public int BeatmapSetId { get; init; }
    public int TotalTime { get; init; }
    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTime).FormatTime();
    public string SongName => GetSongName();
    public string ArtistString => GetArtist();
    public string TitleString => GetTitle();

    public virtual string GetArtist()
    {
        return Artist;
    }

    public virtual string GetTitle()
    {
        return Title;
    }

    public virtual string GetSongName()
    {
        return $"{GetArtist()} - {GetTitle()}";
    }

    public async Task<IMapEntry?> ReadFullEntry()
    {
        if (OsuPath == null) return null;

        var realmLoc = Path.Combine(OsuPath, "client.realm");

        var realmConfig = new RealmConfiguration(realmLoc)
        {
            IsDynamic = true,
            IsReadOnly = true
        };

        var realm = await Realm.GetInstanceAsync(realmConfig);
        var beatmap = (DynamicRealmObject) realm.DynamicApi.Find("BeatmapSet", Id);

        if (beatmap == default) return null;

        var beatmaps = beatmap.DynamicApi.GetList<DynamicRealmObject>(nameof(BeatmapSetInfo.Beatmaps));
        var metadata = beatmaps.First().DynamicApi.Get<DynamicRealmObject>(nameof(BeatmapInfo.Metadata)).DynamicApi;

        var files = (RealmList<DynamicEmbeddedObject>) beatmap.DynamicApi.GetList<DynamicEmbeddedObject>(nameof(BeatmapSetInfo.Files));

        var audioFileName = metadata.Get<string>(nameof(BeatmapMetadata.AudioFile));
        var backgroundFileName = metadata.Get<string>(nameof(BeatmapMetadata.BackgroundFile));

        var audioFile = (IRealmObjectBase) files.FirstOrDefault(x =>
            string.Equals(x.DynamicApi.Get<string>(nameof(RealmNamedFileUsage.Filename)), audioFileName, StringComparison.CurrentCultureIgnoreCase));
        var backgroundFile = (IRealmObjectBase) files.FirstOrDefault(x =>
            string.Equals(x.DynamicApi.Get<string>(nameof(RealmNamedFileUsage.Filename)), backgroundFileName, StringComparison.CurrentCultureIgnoreCase));

        if (audioFile == null) return null;

        var audioHash = audioFile.DynamicApi.Get<IRealmObjectBase>(nameof(RealmNamedFileUsage.File)).DynamicApi.Get<string>(nameof(RealmFile.Hash));
        var backgroundHash = backgroundFile?.DynamicApi.Get<IRealmObjectBase>(nameof(RealmNamedFileUsage.File)).DynamicApi.Get<string>(nameof(RealmFile.Hash));

        var audioFolderName = Path.Combine($"{audioHash[0]}", $"{audioHash[0]}{audioHash[1]}");
        var backgroundFolderName = Path.Combine($"{backgroundHash?[0]}", $"{backgroundHash?[0]}{backgroundHash?[1]}");

        string fullPath;
        string folderPath;

        var folderInfo = new DirectoryInfo(Path.Combine(OsuPath, "files"));

        switch (folderInfo.LinkTarget)
        {
            case { } target:
                fullPath = Path.Combine(target, audioFolderName, audioHash);
                folderPath = Path.Combine(target, audioFolderName);
                break;
            case null:
                fullPath = Path.Combine(OsuPath, "files", audioFolderName, audioHash);
                folderPath = Path.Combine(OsuPath, "files", audioFolderName);
                break;
        }

        var newMap = new RealmMapEntry
        {
            Id = Id,
            OsuPath = string.Intern(OsuPath),
            Artist = string.Intern(Artist),
            ArtistUnicode = metadata.Get<string>(nameof(BeatmapMetadata.ArtistUnicode)),
            Title = Title,
            TitleUnicode = metadata.Get<string>(nameof(BeatmapMetadata.TitleUnicode)),
            AudioFileName = audioFileName,
            BackgroundFileLocation = string.IsNullOrEmpty(backgroundFolderName)
                ? string.Empty
                : Path.Combine(OsuPath, "files", backgroundFolderName, backgroundHash!),
            Hash = Hash,
            BeatmapSetId = BeatmapSetId,
            FolderName = audioFolderName,
            FullPath = fullPath,
            FolderPath = folderPath
        };

        realm.Dispose();

        return newMap;
    }

    public IDatabaseReader? GetReader()
    {
        return OsuPath != null ? new RealmReader(OsuPath) : null;
    }

    public bool Equals(IMapEntryBase? other)
    {
        return Hash == other?.Hash;
    }

    public int CompareTo(IMapEntryBase? other)
    {
        return string.Compare(Hash, other?.Hash, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return GetSongName();
    }

    public static bool operator ==(RealmMapEntryBase? left, IMapEntryBase? right)
    {
        return left?.Hash == right?.Hash;
    }

    public static bool operator !=(RealmMapEntryBase? left, IMapEntryBase? right)
    {
        return left?.Hash != right?.Hash;
    }

    public override bool Equals(object? other)
    {
        if (other is IMapEntryBase map)
            return Hash == map.Hash;

        return false;
    }

    public override int GetHashCode()
    {
        return BitConverter.ToInt32(Encoding.UTF8.GetBytes(Hash));
    }
}