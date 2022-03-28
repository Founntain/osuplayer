namespace OsuPlayer.Extensions.Bindings;

public interface IUnbindable
{
    void UnbindEvents();

    void UnbindBindings();

    void UnbindAll();

    void UnbindFrom(IUnbindable other);
}