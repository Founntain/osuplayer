namespace OsuPlayer.Interfaces.Service;

public interface IOsuPlayerService
{
    protected abstract string ServiceName { get; }
    protected abstract string ServiceTag();
}