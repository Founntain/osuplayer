using System;
using System.Globalization;
using Material.Icons;
using NUnit.Framework;
using OsuPlayer.Extensions.ValueConverters;

namespace OsuPlayer.Tests.ValueConverterTests;

public class VolumeConverterTests
{
    private readonly Type _expectedInput = typeof(double);
    private readonly Type _expectedOutput = typeof(MaterialIconKind);
    private VolumeConverter _playPauseConverter = null!;

    [SetUp]
    public void Setup()
    {
        _playPauseConverter = new VolumeConverter();
    }

    [TestCase(true)]
    [TestCase("test")]
    public void TestWrongInputHandled(object input)
    {
        Assert.That(input, Is.Not.InstanceOf(_expectedInput));
        Assert.DoesNotThrow(() =>
            _playPauseConverter.Convert(input, _expectedOutput, null, CultureInfo.InvariantCulture));
    }

    [Test]
    public void TestNullInputHandled()
    {
        Assert.DoesNotThrow(
            () => _playPauseConverter.Convert(null, _expectedOutput, null, CultureInfo.InvariantCulture));
    }

    [TestCase(new object?[]
    {
        0.0, MaterialIconKind.VolumeMute
    })]
    [TestCase(new object?[]
    {
        10.0, MaterialIconKind.VolumeLow
    })]
    [TestCase(new object?[]
    {
        50.0, MaterialIconKind.VolumeMedium
    })]
    [TestCase(new object?[]
    {
        100.0, MaterialIconKind.VolumeHigh
    })]
    public void TestCorrectUsage(object[] input)
    {
        var volume = input[0];
        var icon = input[1];
        var output = _playPauseConverter.Convert(volume, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.That(output, Is.InstanceOf(_expectedOutput));
        Assert.That(output, Is.EqualTo(icon));
    }

    [Test]
    public void TestOutputOnIncorrectInput()
    {
        var output = _playPauseConverter.Convert(true, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.That(output, Is.InstanceOf(_expectedOutput));
        Assert.That(output, Is.EqualTo(MaterialIconKind.QuestionMark));
    }
}