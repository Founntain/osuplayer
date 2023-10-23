using OsuPlayer.Interfaces.Service;

namespace OsuPlayer.Services;

public class LoggingService : OsuPlayerService, ILoggingService
{
    public override string ServiceName => "LOGGING_SERVICE";

    public void Log(string message, LogType logType = LogType.Info, object? data = null) => LogToConsole(message, logType, data);
}