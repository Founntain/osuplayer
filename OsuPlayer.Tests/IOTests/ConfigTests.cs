using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using OsuPlayer.IO.Storage.Config;

namespace OsuPlayer.Tests.IOTests;

public class ConfigTests
{
    private Config _config = null!;

    [SetUp]
    public void Setup()
    {
        _config = new Config();
        if (Directory.Exists("data"))
            Directory.Delete("data", true);
    }

    [Test]
    public void TestConfigPath()
    {
        Assert.That(Regex.IsMatch(_config.Path, @"^data[\/\\]{1,2}config\.json$"), Is.True);
    }

    [Test]
    public void TestContainerNotNullOnInit()
    {
        Assert.That(_config.Container, Is.Not.Null);
    }

    [Test]
    public void TestContainerNotNullOnRead()
    {
        Assert.DoesNotThrow(() => _config.Read());
        Assert.That(_config.Container, Is.Not.Null);
    }

    [Test]
    public void TestContainerNotNullOnAsyncRead()
    {
        Assert.DoesNotThrowAsync(async () => await _config.ReadAsync());
        Assert.That(_config.Container, Is.Not.Null);
    }

    [Test]
    public void TestSave()
    {
        Assert.DoesNotThrow(() => _config.Save(_config.Read()));
    }

    [Test]
    public void TestAsyncSave()
    {
        Assert.DoesNotThrowAsync(async () => await _config.SaveAsync(await _config.ReadAsync()));
    }

    [Test]
    public void TestCorrectWriteRead()
    {
        var container = _config.Read();
        container.Volume = 10;
        _config.Save(container);
        _config = new Config();
        Assert.That(_config.Read().Volume, Is.EqualTo(10));
    }

    [Test]
    public void TestAccessNoRead()
    {
        Assert.DoesNotThrow(() => _config.Container.Volume = 10);
        Assert.That(_config.Read().Volume, Is.EqualTo(10));
    }
}