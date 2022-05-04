using System.Threading.Tasks;
using NUnit.Framework;
using OsuPlayer.Network.Security;

namespace OsuPlayer.Tests.OsuPlayer.Network;

public class PasswordManagerTests
{
    [Test]
    public void PasswordManagerTest()
    {
        var r1 = PasswordManager.CheckIfPasswordMeetsRequirements("");
        var r2 = PasswordManager.CheckIfPasswordMeetsRequirements("Test");
        var r3 = PasswordManager.CheckIfPasswordMeetsRequirements("Test123!_?");
        
        Assert.IsFalse(r1);
        Assert.IsFalse(r2);
        Assert.IsTrue(r3);
    }
}