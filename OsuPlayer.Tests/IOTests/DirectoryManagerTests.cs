using System.IO;
using NUnit.Framework;
using OsuPlayer.IO;

namespace OsuPlayer.Tests;

public class DirectoryManagerTests
{
    [Test]
    public void GenerateMissingDirectories()
    {
        Assert.DoesNotThrow(() =>
        {
            if (Directory.Exists("data"))
                Directory.Delete("data", true);
        });

        Assert.DoesNotThrow(DirectoryManager.GenerateMissingDirectories);
    }
}