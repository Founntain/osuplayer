using NUnit.Framework;
using OsuPlayer.Network.Security;

namespace OsuPlayer.Tests;

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
    
    [TestCase("", false)]
    [TestCase("Test", false)]
    [TestCase("Test123!_?", true)]
    public void PasswordManagerTestWithErrorList(string pw, bool expected)
    {
        var result = PasswordManager.CheckIfPasswordMeetsRequirementsWithErrorList(pw);

        Assert.AreEqual(result.Item1, expected);
        
        if(!expected)
            Assert.IsTrue(!string.IsNullOrWhiteSpace(result.Item2));
        else
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.Item2));
    }
}