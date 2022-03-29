using System;
using System.Globalization;
using Material.Icons;
using NUnit.Framework;
using OsuPlayer.Extensions.ValueConverters;

namespace OsuPlayer.Tests.ValueConverterTests;

public class PlayPauseConverterTests
{
    private readonly Type _expectedInput = typeof(bool);
    private readonly Type _expectedOutput = typeof(MaterialIconKind);
    private PlayPauseConverter _playPauseConverter;

    [SetUp]
    public void Setup()
    {
        _playPauseConverter = new PlayPauseConverter();
    }

    [TestCase(10)]
    [TestCase("test")]
    public void TestWrongInputHandled(object input)
    {
        Assert.IsNotInstanceOf(_expectedInput, input.GetType());
        Assert.DoesNotThrow(() =>
            _playPauseConverter.Convert(input, _expectedOutput, null, CultureInfo.InvariantCulture));
    }

    [Test]
    public void TestNullInputHandled()
    {
        Assert.DoesNotThrow(
            () => _playPauseConverter.Convert(null, _expectedOutput, null, CultureInfo.InvariantCulture));
    }

    [Test]
    public void TestCorrectUsage()
    {
        var output = _playPauseConverter.Convert(true, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.IsInstanceOf(_expectedOutput, output);
    }

    [Test]
    public void TestOutputOnIncorrectInput()
    {
        var output = _playPauseConverter.Convert(10, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.IsInstanceOf(_expectedOutput, output);
        Assert.AreEqual(output, MaterialIconKind.QuestionMark);
    }
}