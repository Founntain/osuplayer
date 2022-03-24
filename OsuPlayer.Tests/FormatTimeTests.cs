using System;
using NUnit.Framework;
using OsuPlayer.Extensions;

namespace OsuPlayer.Tests;

public class FormatTimeTests
{
    [TestCase(0, 1, 30, "1:30")]
    [TestCase(0, 10, 30, "10:30")]
    [TestCase(1, 1, 30, "1:01:30")]
    public void TestTimeFormat(object[] input)
    {
        var hours = (int)input[0];
        var minutes = (int)input[1];
        var seconds = (int)input[2];
        var expectedString = input[3];
        var actualString = new TimeSpan(hours, minutes, seconds).FormatTime();
        Assert.AreEqual(expectedString, actualString);
    }
}