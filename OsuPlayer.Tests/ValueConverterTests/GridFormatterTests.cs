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
    private GridFormatter _gridFormatter;

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
        Assert.IsNotInstanceOf(_expectedInput, input.GetType());
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
            Assert.IsInstanceOf(_expectedInput, width);
            Assert.IsInstanceOf(_expectedParameter, param);
            Assert.IsInstanceOf(_expectedOutput, expected);
        });
        object output = null;
        Assert.DoesNotThrow(() =>
            output = _gridFormatter.Convert(width, _expectedOutput, param, CultureInfo.InvariantCulture));
        Assert.IsInstanceOf(_expectedOutput, output);
        Assert.AreEqual(expected, output);
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
        Assert.IsInstanceOf(_expectedOutput, output);
        Assert.AreEqual(output, 0.0);
    }
}