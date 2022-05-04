using System.Threading.Tasks;
using NUnit.Framework;
using OsuPlayer.Network;

namespace OsuPlayer.Tests.OsuPlayer.Network;

public class GitHubUpdaterTests
{
    [Test]
    public async Task CheckForUpdatesTest()
    {
        var result = await GitHubUpdater.CheckForUpdates();
        
        Assert.IsFalse(result.Item1);
        
        var result2 = await GitHubUpdater.CheckForUpdates(true);
        
        Assert.IsTrue(result2.Item1);
    }
}