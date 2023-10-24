using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using OsuPlayer.IO.Storage.Config;

namespace OsuPlayer.Tests.IOTests;

public class ConfigTests
{
    private Config _config;

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
        Assert.IsTrue(Regex.IsMatch(_config.Path, @"^data[\/\\]{1,2}config\.json$"));
    }

    [Test]
    public void TestContainerNotNullOnInit()
    {
        Assert.IsNotNull(_config.Container);
    }

    [Test]
    public void TestContainerNotNullOnRead()
    {
        Assert.DoesNotThrow(() => _config.Read());
        Assert.IsNotNull(_config.Container);
    }

    [Test]
    public void TestContainerNotNullOnAsyncRead()
    {
        Assert.DoesNotThrowAsync(async () => await _config.ReadAsync());
        Assert.IsNotNull(_config.Container);
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
        Assert.AreEqual(10, _config.Read().Volume);
    }

    [Test]
    public void TestAccessNoRead()
    {
        Assert.DoesNotThrow(() => _config.Container.Volume = 10);
        Assert.AreEqual(10, _config.Read().Volume);
    }
}