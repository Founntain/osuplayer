using System;
using System.Globalization;
using NUnit.Framework;
using OsuPlayer.Data.DataModels;
using OsuPlayer.Extensions.ValueConverters;

namespace OsuPlayer.Tests.ValueConverterTests;

public class SettingsUserConverterTests
{
    private readonly Type _expectedInput = typeof(User);
    private readonly Type _expectedOutput = typeof(string);
    private SettingsUserConverter _userConverter = null!;

    [SetUp]
    public void Setup()
    {
        _userConverter = new SettingsUserConverter();
    }

    [TestCase(10)]
    [TestCase("test")]
    public void TestWrongInputHandled(object input)
    {
        Assert.That(input, Is.Not.InstanceOf(_expectedInput));
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
        Assert.That(output, Is.EqualTo("Not logged in"));
    }

    // [TestCase("Test")]
    // [TestCase("Hallo")]
    // public void TestCorrectUsage(string name)
    // {
    //     var input = new User
    //     {
    //         Name = name
    //     };
    //
    //     Assert.IsInstanceOf(_expectedInput, input);
    //
    //     var output = _userConverter.Convert(input, _expectedOutput, null, CultureInfo.InvariantCulture);
    //
    //     Assert.IsInstanceOf(_expectedOutput, output);
    //     Assert.AreEqual(input.Name, output);
    // }

    [Test]
    public void TestOutputOnIncorrectInput()
    {
        var output = _userConverter.Convert(10, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.That(output, Is.InstanceOf(_expectedOutput));
        Assert.That(output, Is.EqualTo("Not logged in"));
    }
}