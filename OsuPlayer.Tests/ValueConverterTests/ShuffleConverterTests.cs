using System;
using System.Globalization;
using Material.Icons;
using NUnit.Framework;
using OsuPlayer.Extensions.ValueConverters;

namespace OsuPlayer.Tests.ValueConverterTests;

public class ShuffleConverterTests
{
    private readonly Type _expectedInput = typeof(bool);
    private readonly Type _expectedOutput = typeof(MaterialIconKind);
    private ShuffleConverter _repeatConverter;

    [SetUp]
    public void Setup()
    {
        _repeatConverter = new ShuffleConverter();
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