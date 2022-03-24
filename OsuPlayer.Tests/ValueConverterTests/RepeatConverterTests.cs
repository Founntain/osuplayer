using System;
using System.Globalization;
using Material.Icons;
using NUnit.Framework;
using OsuPlayer.Extensions.ValueConverters;
using OsuPlayer.Network.Online;

namespace OsuPlayer.Tests.ValueConverterTests;

public class RepeatConverterTests
{
    private RepeatConverter _repeatConverter;
    private Type _expectedInput = typeof(bool);
    private Type _expectedOutput = typeof(MaterialIconKind);
    
    [SetUp]
    public void Setup()
    {
        _repeatConverter = new RepeatConverter();
    }

    [TestCase(10)]
    [TestCase("test")]
    public void TestWrongInputHandled(object input)
    {
        Assert.IsNotInstanceOf(_expectedInput, input.GetType());
        Assert.DoesNotThrow(() => _repeatConverter.Convert(input, _expectedOutput, null, CultureInfo.InvariantCulture));
    }

    [Test]
    public void TestNullInputHandled()
    {
        Assert.DoesNotThrow(() => _repeatConverter.Convert(null, _expectedOutput, null, CultureInfo.InvariantCulture));
    }

    [Test]
    public void TestCorrectUsage()
    {
        var output = _repeatConverter.Convert(true, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.IsInstanceOf(_expectedOutput, output);
    }

    [Test]
    public void TestOutputOnIncorrectInput()
    {
        var output = _repeatConverter.Convert(10, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.IsInstanceOf(_expectedOutput, output);
        Assert.AreEqual(output, MaterialIconKind.QuestionMark);
    }
}