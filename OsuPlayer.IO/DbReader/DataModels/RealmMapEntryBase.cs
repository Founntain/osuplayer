﻿using System.Text;
using OsuPlayer.Extensions;
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
    public string Artist { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Hash { get; init; } = string.Empty;
    public int BeatmapSetId { get; set; }
    public int TotalTime { get; set; }
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

    public override string ToString()
    {
        return GetSongName();
    }

    public async Task<IMapEntry?> ReadFullEntry(string path)
    {
        var realmLoc = Path.Combine(path, "client.realm");

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

        var audioFile = (RealmObjectBase) files.FirstOrDefault(x => string.Equals(x.DynamicApi.Get<string>(nameof(RealmNamedFileUsage.Filename)), audioFileName, StringComparison.CurrentCultureIgnoreCase));
        var backgroundFile = (RealmObjectBase) files.FirstOrDefault(x => string.Equals(x.DynamicApi.Get<string>(nameof(RealmNamedFileUsage.Filename)), backgroundFileName, StringComparison.CurrentCultureIgnoreCase));

        if (audioFile == null || backgroundFile == null) return null;

        var audioHash = audioFile.DynamicApi.Get<RealmObjectBase>(nameof(RealmNamedFileUsage.File)).DynamicApi.Get<string>(nameof(RealmFile.Hash));
        var backgroundHash = backgroundFile.DynamicApi.Get<RealmObjectBase>(nameof(RealmNamedFileUsage.File)).DynamicApi.Get<string>(nameof(RealmFile.Hash));

        var audioFolderName = Path.Combine($"{audioHash[0]}", $"{audioHash[0]}{audioHash[1]}");
        var backgroundFolderName = Path.Combine($"{backgroundHash[0]}", $"{backgroundHash[0]}{backgroundHash[1]}");

        var newMap = new RealmMapEntry
        {
            Id = Id,
            Artist = Artist,
            ArtistUnicode = metadata.Get<string>(nameof(BeatmapMetadata.ArtistUnicode)),
            Title = Title,
            TitleUnicode = metadata.Get<string>(nameof(BeatmapMetadata.TitleUnicode)),
            AudioFileName = audioFileName,
            BackgroundFileLocation = string.IsNullOrEmpty(backgroundFolderName) ? string.Empty : Path.Combine(path, "files", backgroundFolderName, backgroundHash),
            Hash = Hash,
            BeatmapSetId = BeatmapSetId,
            FolderName = audioFolderName,
            FolderPath = Path.Combine("files", audioFolderName),
            FullPath = Path.Combine(path, "files", audioFolderName, audioHash)
        };

        realm.Dispose();

        return newMap;
    }

    public IDatabaseReader GetReader(string path)
    {
        return new RealmReader(path);
    }

    public static bool operator ==(RealmMapEntryBase? left, IMapEntryBase? right)
    {
        return left?.Hash == right?.Hash;
    }

    public static bool operator !=(RealmMapEntryBase? left, IMapEntryBase? right)
    {
        return left?.Hash != right?.Hash;
    }

    public bool Equals(IMapEntryBase? other)
    {
        return Hash == other?.Hash;
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