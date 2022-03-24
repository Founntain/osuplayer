using System;
using System.Globalization;
using Material.Icons;
using NUnit.Framework;
using OsuPlayer.Extensions.ValueConverters;

namespace OsuPlayer.Tests.ValueConverterTests;

public class GridFormatterTests
{
    private GridFormatter _gridFormatter;
    private Type _expectedInput = typeof(double);
    private Type _expectedParameter = typeof(string);
    private Type _expectedOutput = typeof(int);

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

    [TestCase(new object[] {100.0, "100", 1})]
    [TestCase(new object[] {100.0, "99", 2})]
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

    [TestCase(new object[] {"test", 100})]
    [TestCase(new object[] {10, true})]
    [TestCase(new object[] {"abc", 0.0})]
    public void TestOutputOnIncorrectInput(object[] input)
    {
        var width = input[0];
        var param = input[1];
        object? output = null;
        Assert.DoesNotThrow(() =>
            output = _gridFormatter.Convert(width, _expectedOutput, param, CultureInfo.InvariantCulture));
        Assert.IsInstanceOf(_expectedOutput, output);
        Assert.AreEqual(output, 0);
    }
}