using NUnit.Framework;

namespace OsuPlayer.Tests;

public class ConstantsTests
{
    [Test]
    public void ConstantsTest()
    {
        Network.Constants.Localhost = true;
        
        var setLocalhost = Network.Constants.Localhost;

        Assert.IsTrue(Network.Constants.Localhost);
        Assert.IsTrue(setLocalhost);
        
        Network.Constants.OfflineMode = true;
        
        var setOfflineMode = Network.Constants.OfflineMode;

        Assert.IsTrue(Network.Constants.OfflineMode);
        Assert.IsTrue(setOfflineMode);
    }
}