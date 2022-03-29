namespace OsuPlayer.IO.Storage.LazerModels.Interfaces;

public interface IDeepCloneable<out T> where T : class
{
    /// <summary>
    /// Creates a new <typeparamref name="T" /> that is a deep copy of the current instance.
    /// </summary>
    /// <returns>The <typeparamref name="T" />.</returns>
    T DeepClone();
}