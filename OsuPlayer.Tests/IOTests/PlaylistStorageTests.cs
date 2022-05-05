using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.IO.Storage.Playlists;

namespace OsuPlayer.Tests;

public class PlaylistStorageTests
{
    private PlaylistStorage _playlist;

    [SetUp]
    public void Setup()
    {
        _playlist = new PlaylistStorage();

        if (Directory.Exists("data"))
            Directory.Delete("data", true);
    }

    [Test]
    public void TestPlaylistsPath()
    {
        Assert.IsTrue(Regex.IsMatch(_playlist.Path, @"^data[\/\\]{1,2}playlists\.json$"));
    }

    [Test]
    public void TestContainerNotNullOnInit()
    {
        Assert.IsNotNull(_playlist.Container);
    }

    [Test]
    public void TestContainerNotNullOnRead()
    {
        _playlist.Read();
        Assert.IsNotNull(_playlist.Container);
    }

    [Test]
    public void TestContainerNotNullOnAsyncRead()
    {
        Assert.DoesNotThrowAsync(async () => await _playlist.ReadAsync());
        Assert.IsNotNull(_playlist.Container);
    }

    [Test]
    public void TestSave()
    {
        Assert.DoesNotThrow(() => _playlist.Save(_playlist.Read()));
    }

    [Test]
    public void TestAsyncSave()
    {
        Assert.DoesNotThrowAsync(async () => await _playlist.SaveAsync(await _playlist.ReadAsync()));
    }

    [Test]
    public void TestCorrectWriteRead()
    {
        var container = _playlist.Read();
        var testPlaylist = new List<Playlist>
        {
            new()
            {
                Name = "Test"
            }
        };

        container.Playlists = testPlaylist;

        _playlist.Save(container);
        _playlist = new PlaylistStorage();

        var newContainer = _playlist.Read();
        var sequenceEqual =
            testPlaylist.Select(x => x.Id).SequenceEqual(newContainer.Playlists!.Select(x => x.Id));

        Assert.IsTrue(sequenceEqual);
    }

    [Test]
    public void TestAccessNoRead()
    {
        var testPlaylist = new List<Playlist>
        {
            new()
        };

        Assert.DoesNotThrow(() => _playlist.Container.Playlists = testPlaylist);
        Assert.AreEqual(testPlaylist, _playlist.Read().Playlists);
    }

    [Test]
    public void StorageContainerSetTest()
    {
        _playlist.Container = new PlaylistContainer().Init();
    }
}