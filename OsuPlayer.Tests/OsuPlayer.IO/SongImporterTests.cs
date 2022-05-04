using System.Threading.Tasks;
using NUnit.Framework;
using OsuPlayer.IO;

namespace OsuPlayer.Tests;

public class SongImporterTests
{
    [Test]
    public async Task ImporterTests()
    {
        //We can't run an actual import, because what path should be use obviously.
        //So we are running negative tests here :(
        
        await SongImporter.ImportSongsAsync(string.Empty);
        await SongImporter.ImportSongsAsync("U:\\HOPEFULLYNOTEXISTINGPATH");
    }
}