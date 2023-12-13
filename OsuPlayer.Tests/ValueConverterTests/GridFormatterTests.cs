using System;
using System.Globalization;
using NUnit.Framework;
using OsuPlayer.Extensions.ValueConverters;

namespace OsuPlayer.Tests.ValueConverterTests;

public class GridFormatterTests
{
    private readonly Type _expectedInput = typeof(double);
    private readonly Type _expectedOutput = typeof(double);
    private readonly Type _expectedParameter = typeof(string);
    private GridFormatter _gridFormatter = null!;

    [SetUp]
    public void Setup()
    {
        _gridFormatter = new GridFormatter();
    }

    [TestCase(10)]
    [TestCase("test")]
    [TestCase(true)]
    public void TestWrongInputHandled(object input)
    {
        Assert.That(input, Is.Not.InstanceOf(_expectedInput));
        Assert.DoesNotThrow(() =>
            _gridFormatter.Convert(input, _expectedOutput, input, CultureInfo.InvariantCulture));
    }

    [Test]
    public void TestNullInputHandled()
    {
        Assert.DoesNotThrow(
            () => _gridFormatter.Convert(null, _expectedOutput, null, CultureInfo.InvariantCulture));
    }

    [TestCase(new object[]
    {
        100.0, "100", 100.0
    })]
    [TestCase(new object[]
    {
        150.0, "100", 75.0
    })]
    public void TestCorrectUsage(object[] input)
    {
        var width = input[0];
        var param = input[1];
        var expected = input[2];
        Assert.Multiple(() =>
        {
            Assert.That(width, Is.InstanceOf(_expectedInput));
            Assert.That(param, Is.InstanceOf(_expectedParameter));
            Assert.That(expected, Is.InstanceOf(_expectedOutput));
        });
        object output = null;
        Assert.DoesNotThrow(() =>
            output = _gridFormatter.Convert(width, _expectedOutput, param, CultureInfo.InvariantCulture));
        Assert.That(output, Is.InstanceOf(_expectedOutput));
        Assert.That(output, Is.EqualTo(expected));
    }

    [TestCase(new object[]
    {
        "test", 100
    })]
    [TestCase(new object[]
    {
        10, true
    })]
    [TestCase(new object[]
    {
        "abc", 0.0
    })]
    public void TestOutputOnIncorrectInput(object[] input)
    {
        var width = input[0];
        var param = input[1];
        object? output = null;
        Assert.DoesNotThrow(() =>
            output = _gridFormatter.Convert(width, _expectedOutput, param, CultureInfo.InvariantCulture));
        Assert.That(output, Is.InstanceOf(_expectedOutput));
        Assert.That(output, Is.EqualTo(0.0));
    }
}