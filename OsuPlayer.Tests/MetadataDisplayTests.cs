using NUnit.Framework;
using OsuPlayer.Data.DataModels;

namespace OsuPlayer.Tests;

public class MetadataDisplayTests
{
    [Test]
    public void DisplayMetadataPrefersUnicodeWithoutChangingRawMetadata()
    {
        var song = new RealmMapEntryBase
        {
            DbReaderFactory = null!,
            Artist = "Yuaru",
            ArtistUnicode = "ゆある",
            Title = "Asu no Yozora Shoukaihan",
            TitleUnicode = "アスノヨゾラ哨戒班",
            UseUnicode = false
        };

        Assert.Multiple(() =>
        {
            Assert.That(song.Title, Is.EqualTo("Asu no Yozora Shoukaihan"));
            Assert.That(song.Artist, Is.EqualTo("Yuaru"));
            Assert.That(song.GetDisplayTitle(), Is.EqualTo("アスノヨゾラ哨戒班"));
            Assert.That(song.GetDisplayArtist(), Is.EqualTo("ゆある"));
            Assert.That(song.TitleString, Is.EqualTo("アスノヨゾラ哨戒班"));
            Assert.That(song.ArtistString, Is.EqualTo("ゆある"));
        });
    }

    [Test]
    public void DisplayMetadataFallsBackWhenUnicodeIsEmpty()
    {
        var song = new RealmMapEntryBase
        {
            DbReaderFactory = null!,
            Artist = "Green Day",
            Title = "21 Guns",
            ArtistUnicode = " ",
            TitleUnicode = string.Empty
        };

        Assert.Multiple(() =>
        {
            Assert.That(song.GetDisplayTitle(), Is.EqualTo("21 Guns"));
            Assert.That(song.GetDisplayArtist(), Is.EqualTo("Green Day"));
            Assert.That(song.DisplaySongName, Is.EqualTo("Green Day - 21 Guns"));
        });
    }
}
