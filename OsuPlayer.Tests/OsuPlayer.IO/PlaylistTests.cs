using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.IO.Storage.Playlists;

namespace OsuPlayer.Tests;

public class PlaylistTests
{
    [Test]
    public void SetCurrentPlaylistTest()
    {
        PlaylistManager.SetCurrentPlaylist(default);
        PlaylistManager.SetCurrentPlaylist(new ());
    }
    
    [Test]
    public async Task SetCurrentPlaylistAsyncTest()
    {
        await PlaylistManager.SetCurrentPlaylistAsync(default);
        await PlaylistManager.SetCurrentPlaylistAsync(new ());
    }

    [Test]
    public async Task GetAllPlaylists()
    {
        PlaylistManager.GetAllPlaylists();
        await PlaylistManager.GetAllPlaylistsAsync();
    }

    [Test]
    public async Task AddPlaylistAsync()
    {
        var id = Guid.NewGuid();
        
        await PlaylistManager.AddPlaylistAsync(new()
        {
            Id = id,
            Name = "Unit Test"
        });
        
        await PlaylistManager.AddPlaylistAsync(new()
        {
            Id = id,
            Name = "Unit Test"
        });
    }

    [Test]
    public async Task ReplacePlaylistTest()
    {
        await PlaylistManager.ReplacePlaylistAsync(new()
        {
            Name = "Favorites"
        });
        
        await PlaylistManager.ReplacePlaylistAsync(default);

        await PlaylistManager.ReplacePlaylistsAsync(new List<Playlist>()
        {
            new()
            {
                Name = "Favorites"
            }
        });
    }

    [Test]
    public async Task RenamePlaylist()
    {
        var id = Guid.NewGuid();

        var playlist = new Playlist()
        {
            Id = id,
            Name = id.ToString()
        };

        await PlaylistManager.AddPlaylistAsync(playlist);

        playlist.Name = "New Name";

        await PlaylistManager.RenamePlaylist(playlist);
        
        var playlists = await PlaylistManager.GetAllPlaylistsAsync();
        
        Assert.IsTrue(playlists.Any(x => x.Name == "New Name"));
    }

    [Test]
    public async Task SavePlaylist()
    {
        await PlaylistManager.SavePlaylistsAsync();
    }

    [Test]
    public async Task DeletePlaylist()
    {
        var id = Guid.NewGuid();

        var playlist = new Playlist()
        {
            Id = id,
            Name = id.ToString()
        };

        await PlaylistManager.AddPlaylistAsync(playlist);

        await PlaylistManager.DeletePlaylistAsync(playlist);

        var playlists = await PlaylistManager.GetAllPlaylistsAsync();
        
        Assert.IsFalse(playlists.Any(x => x.Name == id.ToString()));
    }

    [Test]
    public void SetLastKnownPlaylistAsCurrentPlaylist()
    {
        PlaylistManager.SetLastKnownPlaylistAsCurrentPlaylist();
    }

    [Test]
    public void CreateContainer()
    {
        var obj = new PlaylistContainer().Init();
        
        Assert.IsNotNull(obj);
    }
}