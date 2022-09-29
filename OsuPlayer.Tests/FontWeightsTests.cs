using NUnit.Framework;
using OsuPlayer.Extensions;
using OsuPlayer.Extensions.Enums;

namespace OsuPlayer.Tests;

public class FontWeightsTests
{
    [TestCase(FontWeights.Light)]
    [TestCase(FontWeights.Regular)]
    [TestCase(FontWeights.Medium)]
    [TestCase(FontWeights.Bold)]
    [TestCase(FontWeights.ExtraBold)]
    public void NextHigherFontTest(FontWeights input)
    {
        var result = input.GetNextHigherFont();
        
        if(input == FontWeights.ExtraBold)
            Assert.IsTrue(result == input);
        else
            Assert.IsTrue(result != input);
    }
    
    [TestCase(FontWeights.Light)]
    [TestCase(FontWeights.Regular)]
    [TestCase(FontWeights.Medium)]
    [TestCase(FontWeights.Bold)]
    [TestCase(FontWeights.ExtraBold)]
    public void NextSmallerFontTest(FontWeights input)
    {
        var result = input.GetNextSmallerFont();
        
        if(input == FontWeights.Light)
            Assert.IsTrue(result == input);
        else
            Assert.IsTrue(result != input);
    }
}