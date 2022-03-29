using System;
using System.Collections.Generic;
using System.Globalization;
using DynamicData;
using NUnit.Framework;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Extensions.ValueConverters;

namespace OsuPlayer.Tests.ValueConverterTests;

public class SourceListValueConverterTests
{
    private readonly Type _expectedInput = typeof(SourceList<Playlist>);
    private readonly Type _expectedOutput = typeof(List<Playlist>);
    private SourceListValueConverter _playPauseConverter;

    [SetUp]
    public void Setup()
    {
        _playPauseConverter = new SourceListValueConverter();
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
        var input = new SourceList<Playlist>();
        input.Add(new Playlist());
        var initialItemCount = input.Count;
        Assert.IsInstanceOf(_expectedInput, input);
        var output = _playPauseConverter.Convert(input, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.IsInstanceOf(_expectedOutput, output);
        Assert.AreEqual(initialItemCount, ((List<Playlist>)output)!.Count);
    }

    [Test]
    public void TestOutputOnIncorrectInput()
    {
        var output = _playPauseConverter.Convert(10, _expectedOutput, null, CultureInfo.InvariantCulture);
        Assert.IsInstanceOf(_expectedOutput, output);
    }
}