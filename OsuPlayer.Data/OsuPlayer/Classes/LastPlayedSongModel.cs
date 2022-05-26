namespace OsuPlayer.Data.OsuPlayer.Classes;

/// <summary>
/// Represents a record for the last played song.
/// The <see cref="SetId" /> is mainly used for songs, but <see cref="Title" /> and
/// <see cref="Artist" /> is available for fallback if the SetId is -1 for non-online maps
/// </summary>
/// <param name="SetId">The beatmap set id of the song</param>
/// <param name="Title">The title of the song</param>
/// <param name="Artist">The artist of the song</param>
public record LastPlayedSongModel(int SetId, string Title, string Artist);