using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using OsuPlayer.IO.Storage.Config;

namespace OsuPlayer.Tests;

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
        Assert.AreEqual("data/config.json", _config.Path);
    }

    [Test]
    public void TestContainerNotNullOnInit()
    {
        Assert.IsNotNull(_config.Container);
    }

    [Test]
    public void TestContainerNotNullOnRead()
    {
        _config.Read();
        Assert.IsNotNull(_config.Container);
    }

    [Test]
    public async Task TestContainerNotNullOnAsyncRead()
    {
        await _config.ReadAsync();
        Assert.IsNotNull(_config.Container);
    }

    [Test]
    public void TestSave()
    {
        _config.Save(_config.Read());
    }

    [Test]
    public async Task TestAsyncSave()
    {
        await _config.SaveAsync(await _config.ReadAsync());
    }

    [Test]
    public void TestCorrectWriteRead()
    {
        var container = _config.Read();
        container.Volume = 10;
        _config.Save(container);
        Assert.AreEqual(10, _config.Read().Volume);
    }

    [Test]
    public void TestAccessNoRead()
    {
        Assert.DoesNotThrow(() => _config.Container.Volume = 10);
        Assert.AreEqual(10, _config.Read().Volume);
    }
}