namespace OsuPlayer.Services.Interfaces;

public interface IOsuPlayerService
{
    protected abstract string ServiceName { get; }
    protected abstract string ServiceTag();
}