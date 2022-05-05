using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using OsuPlayer.IO;

namespace OsuPlayer.Tests;

public class SongImporterTests
{
    [Test]
    public async Task ImporterEmptyTest()
    {
        //We can't run an actual import, because what path should be use obviously.
        //So we are running negative tests here :(

        await SongImporter.ImportSongsAsync(string.Empty);
    }

    [Test]
    public async Task ImporterTest()
    {
        //We can't run an actual import, because what path should be use obviously.
        //So we are running negative tests here :(

        await SongImporter.ImportSongsAsync(Path.Combine("U", "HOPEFULLYNOTEXISTINGPATH"));
    }
}