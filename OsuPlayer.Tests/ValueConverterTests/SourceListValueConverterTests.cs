using System;
using System.Collections.Generic;
using System.Globalization;
using DynamicData;
using NUnit.Framework;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.Extensions.ValueConverters;

namespace OsuPlayer.Tests.ValueConverterTests;

public class SourceListValueConverterTests
{
    private readonly Type _expectedInput = typeof(SourceList<Playlist>);
    private readonly Type _expectedOutput = typeof(List<Playlist>);
    private SourceListValueConverter _playPauseConverter = null!;

    [SetUp]
    public void Setup()
    {
        _playPauseConverter = new SourceListValueConverter();
    }

    [TestCase(10)]
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

    [Test]
    public void TestCorrectUsage()
    {
        var input = new SourceList<Playlist>();
        input.Add(new Playlist());
        var initialItemCount = input.Count;
        Assert.That(input, Is.InstanceOf(_expectedInput));
        var output = _playPauseConverter.Convert(input, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.That(output, Is.InstanceOf(_expectedOutput));
        Assert.That(((List<Playlist>) output)!.Count, Is.EqualTo(initialItemCount));
    }

    [Test]
    public void TestOutputOnIncorrectInput()
    {
        var output = _playPauseConverter.Convert(10, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.That(output, Is.InstanceOf(_expectedOutput));
    }
}