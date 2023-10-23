using NUnit.Framework;
using OsuPlayer.Network.Security;

namespace OsuPlayer.Tests.NetworkTests;

public class PasswordManagerTests
{
    [TestCase("", false)]
    [TestCase("Test", false)]
    [TestCase("Test123!_?", true)]
    public void PasswordManagerTest(string pw, bool expected)
    {
        var result = PasswordManager.CheckIfPasswordMeetsRequirements(pw);

        Assert.AreEqual(result, expected);
    }
}