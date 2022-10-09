namespace OsuPlayer.Data.OsuPlayer.StorageModels;

/// <summary>
/// This interface represents a implementing type which is given to <see cref="Storable{T}" />
/// </summary>
public interface IStorableContainer
{
    public IStorableContainer Init();
}