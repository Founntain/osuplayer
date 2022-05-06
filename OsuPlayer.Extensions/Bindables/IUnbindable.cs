namespace OsuPlayer.Extensions.Bindables;

public interface IUnbindable
{
    void UnbindEvents();

    void UnbindBindings();

    void UnbindAll();

    void UnbindFrom(IUnbindable other);
}