using System;
using System.Globalization;
using Material.Icons;
using NUnit.Framework;
using OsuPlayer.Extensions.ValueConverters;
using OsuPlayer.Network.Online;

namespace OsuPlayer.Tests.ValueConverterTests;

public class SettingsUserConverterTests
{
    private SettingsUserConverter _userConverter;
    private Type _expectedInput = typeof(User);
    private Type _expectedOutput = typeof(string);
    
    [SetUp]
    public void Setup()
    {
        _userConverter = new SettingsUserConverter();
    }

    [TestCase(10)]
    [TestCase("test")]
    public void TestWrongInputHandled(object input)
    {
        Assert.IsNotInstanceOf(_expectedInput, input);
        Assert.DoesNotThrow(() => _userConverter.Convert(10, _expectedOutput, null, CultureInfo.InvariantCulture));
    }

    [Test]
    public void TestNullInputHandled()
    {
        Assert.DoesNotThrow(() => _userConverter.Convert(null, _expectedOutput, null, CultureInfo.InvariantCulture));
    }
    
    [Test]
    public void TestOutputOnNullInput()
    {
        var output = _userConverter.Convert(null, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.AreEqual("Not logged in", output);
    }

    [TestCase("Test")]
    [TestCase("Hallo")]
    public void TestCorrectUsage(string name)
    {
        var input = new User
        {
            Name = name
        };
        Assert.IsInstanceOf(_expectedInput, input);
        var output = _userConverter.Convert(input, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.IsInstanceOf(_expectedOutput, output);
        Assert.AreEqual(input.Name, output);
    }

    [Test]
    public void TestOutputOnIncorrectInput()
    {
        var output = _userConverter.Convert(10, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.IsInstanceOf(_expectedOutput, output);
        Assert.AreEqual(output, "Wrong converter usage");
    }
}