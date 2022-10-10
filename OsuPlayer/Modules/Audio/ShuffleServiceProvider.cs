using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Extensions;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Modules.ShuffleImpl;
using Splat;

namespace OsuPlayer.Modules.Audio;

public class ShuffleServiceProvider : IShuffleServiceProvider
{
    public List<ShuffleAlgorithm> ShuffleAlgorithms { get; }
    public IShuffleImpl? ShuffleImpl { get; private set; }

    public ShuffleServiceProvider()
    {
        using var config = new Config();
        var shuffleType = typeof(IShuffleImpl);

        ShuffleAlgorithms = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => shuffleType.IsAssignableFrom(p)).Select(x => new ShuffleAlgorithm(x)).ToList();
        ShuffleAlgorithms.RemoveAll(x => x.Type == shuffleType);

        var shuffleAlgo = ShuffleAlgorithms.FirstOrDefault(x => x.Type.Name == config.Container.ShuffleAlgorithm);

        if (shuffleAlgo != null)
        {
            Locator.CurrentMutable.UnregisterAll<IShuffleImpl>();
            Locator.CurrentMutable.RegisterLazySingleton(() => Activator.CreateInstance(shuffleAlgo.Type) as IShuffleImpl);
        }
        else
        {
            var defaultShuffle = ShuffleAlgorithms.FirstOrDefault(x => x.Type.IsDefined(typeof(DefaultImplAttr), false));

            Locator.CurrentMutable.UnregisterAll<IShuffleImpl>();
            if (defaultShuffle != null)
                Locator.CurrentMutable.RegisterLazySingleton(() => Activator.CreateInstance(defaultShuffle.Type) as IShuffleImpl);

            config.Container.ShuffleAlgorithm = defaultShuffle?.Type.Name;
        }

        ShuffleImpl = Locator.Current.GetService<IShuffleImpl>();
    }

    public void SetShuffleImpl(ShuffleAlgorithm? algorithm)
    {
        using var config = new Config();
        config.Container.ShuffleAlgorithm = algorithm?.Type.Name;

        Locator.CurrentMutable.UnregisterAll<IShuffleImpl>();

        if (algorithm?.Type != null)
            Locator.CurrentMutable.RegisterLazySingleton(() => Activator.CreateInstance(algorithm.Type) as IShuffleImpl);

        ShuffleImpl = Locator.Current.GetService<IShuffleImpl>();
    }
}