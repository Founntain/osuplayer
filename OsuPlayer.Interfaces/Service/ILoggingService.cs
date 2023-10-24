using OsuPlayer.Services;

namespace OsuPlayer.Interfaces.Service;

public interface ILoggingService
{
    public void Log(string message, LogType logType = LogType.Info, object? data = null);
}