using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.IO.Storage.Playlists;

namespace OsuPlayer.Tests.IOTests;

public class PlaylistStorageTests
{
    private PlaylistStorage _playlist = null!;

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
        Assert.That(Regex.IsMatch(_playlist.Path, @"^data[\/\\]{1,2}playlists\.json$"), Is.True);
    }

    [Test]
    public void TestContainerNotNullOnInit()
    {
        Assert.That(_playlist.Container, Is.Not.Null);
    }

    [Test]
    public void TestContainerNotNullOnRead()
    {
        _playlist.Read();
        Assert.That(_playlist.Container, Is.Not.Null);
    }

    [Test]
    public void TestContainerNotNullOnAsyncRead()
    {
        Assert.DoesNotThrowAsync(async () => await _playlist.ReadAsync());
        Assert.That(_playlist.Container, Is.Not.Null);
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

        Assert.That(sequenceEqual, Is.True);
    }

    [Test]
    public void TestAccessNoRead()
    {
        var testPlaylist = new List<Playlist>
        {
            new()
        };

        Assert.DoesNotThrow(() => _playlist.Container.Playlists = testPlaylist);
        Assert.That(_playlist.Read().Playlists, Is.EqualTo(testPlaylist));
    }

    [Test]
    public void StorageContainerSetTest()
    {
        _playlist.Container = (PlaylistContainer) new PlaylistContainer().Init();
    }
}