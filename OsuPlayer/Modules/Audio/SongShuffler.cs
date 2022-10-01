using System.Diagnostics;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.IO.Storage.Playlists;

namespace OsuPlayer.Modules.Audio;

public class SongShuffler
{
    private int _shuffleHistoryIndex;
    private readonly int?[] _shuffleHistory = new int?[10];
    private readonly IPlayer _player;

    public SongShuffler(IPlayer player)
    {
        _player = player;
    }

    /// <summary>
    /// Implements the shuffle logic <seealso cref="GetNextShuffledIndex" />
    /// </summary>
    /// <param name="direction">the <see cref="ShuffleDirection" /> to shuffle to</param>
    /// <returns>a random/shuffled <see cref="IMapEntryBase" /></returns>
    public IMapEntryBase? DoShuffle(ShuffleDirection direction)
    {
        if (_player.CurrentSong.Value == default || _player.SongSourceList == default)
            throw new NullReferenceException();

        if (_player.RepeatMode.Value == RepeatMode.Playlist && _player.ActivePlaylist == default)
        {
            _player.ActivePlaylistId = (PlaylistManager.GetAllPlaylists() as List<Playlist>)?.Find(x => x.Name == "Favorites")?.Id;
        }

        switch (direction)
        {
            case ShuffleDirection.Forward:
            {
                // Next index if not at array end
                if (_shuffleHistoryIndex < _shuffleHistory.Length - 1)
                {
                    GetNextShuffledIndex();
                }
                // Move array one down if at the top of the array
                else
                {
                    Array.Copy(_shuffleHistory, 1, _shuffleHistory, 0, _shuffleHistory.Length - 1);

                    _shuffleHistory[_shuffleHistoryIndex] = GenerateShuffledIndex();
                }

                break;
            }
            case ShuffleDirection.Backwards:
            {
                // Prev index if index greater than zero
                if (_shuffleHistoryIndex > 0)
                {
                    GetPreviousShuffledIndex();
                }
                // Move array one up if at the start of the array
                else
                {
                    Array.Copy(_shuffleHistory, 0, _shuffleHistory, 1, _shuffleHistory.Length - 1);

                    _shuffleHistory[_shuffleHistoryIndex] = GenerateShuffledIndex();
                }

                break;
            }
        }

        Debug.WriteLine("ShuffleHistory: " + _shuffleHistoryIndex);

        // ReSharper disable once PossibleInvalidOperationException
        var shuffleIndex = (int) _shuffleHistory[_shuffleHistoryIndex];

        return _player.RepeatMode.Value == RepeatMode.Playlist && _player.ActivePlaylist != default
            ? _player.GetMapEntryFromHash(_player.ActivePlaylist!.Songs[shuffleIndex])
            : _player.SongSourceList![shuffleIndex];
    }

    /// <summary>
    /// Generates the next shuffled index in <see cref="_shuffleHistory" />
    /// <seealso cref="GenerateShuffledIndex" />
    /// </summary>
    private void GetNextShuffledIndex()
    {
        // If there is no "next" song generate new shuffled index
        if (_shuffleHistory[_shuffleHistoryIndex + 1] == null)
        {
            _shuffleHistory[_shuffleHistoryIndex] = _player.RepeatMode.Value == RepeatMode.Playlist
                ? _player.ActivePlaylist?.Songs.IndexOf(_player.CurrentSong.Value!.Hash)
                : _player.CurrentIndex;
            _shuffleHistory[++_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
        // There is a "next" song in the history
        else
        {
            // Check if next song index is in allowed boundary
            if (_shuffleHistory[_shuffleHistoryIndex + 1] < (_player.RepeatMode.Value == RepeatMode.Playlist
                    ? _player.ActivePlaylist?.Songs.Count
                    : _player.SongSourceList!.Count))
                _shuffleHistoryIndex++;
            // Generate new shuffled index when not
            else
                _shuffleHistory[++_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
    }

    /// <summary>
    /// Generates the previous shuffled index in <see cref="_shuffleHistory" />
    /// <seealso cref="GenerateShuffledIndex" />
    /// </summary>
    private void GetPreviousShuffledIndex()
    {
        // If there is no "prev" song generate new shuffled index
        if (_shuffleHistory[_shuffleHistoryIndex - 1] == null)
        {
            _shuffleHistory[_shuffleHistoryIndex] = _player.RepeatMode.Value == RepeatMode.Playlist
                ? _player.ActivePlaylist?.Songs.IndexOf(_player.CurrentSong.Value!.Hash)
                : _player.CurrentIndex;
            _shuffleHistory[--_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
        // There is a "prev" song in history
        else
        {
            // Check if next song index is in allowed boundary
            if (_shuffleHistory[_shuffleHistoryIndex - 1] < (_player.RepeatMode.Value == RepeatMode.Playlist
                    ? _player.ActivePlaylist?.Songs.Count
                    : _player.SongSourceList!.Count))
                _shuffleHistoryIndex--;
            // Generate new shuffled index when not
            else
                _shuffleHistory[--_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
    }

    /// <summary>
    /// Generates a new random/shuffled index of available songs in either the <see cref="SongSourceList" /> or
    /// <see cref="ActivePlaylist" /> songs
    /// </summary>
    /// <returns>the index of the new shuffled index</returns>
    private int GenerateShuffledIndex()
    {
        var rdm = new Random();
        var shuffleIndex = rdm.Next(0, _player.RepeatMode.Value == RepeatMode.Playlist
            ? _player.ActivePlaylist!.Songs.Count
            : _player.SongSourceList!.Count);

        while (shuffleIndex == (_player.RepeatMode.Value == RepeatMode.Playlist
                   ? _player.ActivePlaylist?.Songs.IndexOf(_player.CurrentSong.Value!.Hash)
                   : _player.CurrentIndex)) // || OsuPlayer.Blacklist.IsSongInBlacklist(Songs[shuffleIndex]))
            shuffleIndex = rdm.Next(0, _player.RepeatMode.Value == RepeatMode.Playlist
                ? _player.ActivePlaylist!.Songs.Count
                : _player.SongSourceList!.Count);

        return shuffleIndex;
    }
}