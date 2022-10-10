using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Modules.ShuffleImpl;

namespace OsuPlayer.Modules.Services;

public interface IShuffleServiceProvider
{
    public List<ShuffleAlgorithm> ShuffleAlgorithms { get; }
    public IShuffleImpl? ShuffleImpl { get; }
    
    public void SetShuffleImpl(ShuffleAlgorithm? algorithm);
}