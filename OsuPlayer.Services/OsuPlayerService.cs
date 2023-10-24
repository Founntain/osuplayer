using System.Text;
using System.Text.Json;
using OsuPlayer.Interfaces.Service;

namespace OsuPlayer.Services;

public abstract class OsuPlayerService : IOsuPlayerService
{
    public abstract string ServiceName { get; }

    protected OsuPlayerService()
    {
        try
        {
            Console.OutputEncoding = Encoding.UTF8;

            LogToConsole("Service initialized.", LogType.Success, includeLogTypeTag: false);
        }
        catch (Exception _)
        {
            LogToConsole($"Service initialized, but couldn't initialize with encoding {Encoding.UTF8.EncodingName}.", LogType.Warning, includeLogTypeTag: false);
        }
    }

    ~OsuPlayerService() => LogToConsole("Service deinitialized.", LogType.Warning, includeLogTypeTag: false);

    public string ServiceTag() => $"[{ServiceName}] ";

    protected void LogToConsole(string message, LogType logType = LogType.Info, object? data = null, bool includeLogTypeTag = true)
    {
        string outputMessage;

        if (data == null)
            outputMessage = $"{ServiceTag()}{(includeLogTypeTag ? GetLogTypeIcon(logType) + " " : string.Empty)}{message}";
        else
            outputMessage =
                $"{ServiceTag()}{(includeLogTypeTag ? GetLogTypeIcon(logType) + " " : string.Empty)}{message} - {JsonSerializer.Serialize(data)}";

        switch (logType)
        {
            case LogType.Info:
                Console.ResetColor();

                break;
            case LogType.Success:
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.White;

                break;
            case LogType.Warning:
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.ForegroundColor = ConsoleColor.White;

                break;
            case LogType.Error:
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;

                break;
            case LogType.Debug:
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.ForegroundColor = ConsoleColor.White;

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
        }

        Console.WriteLine(outputMessage);

        // Resetting the colors, so subsequent logs don't have the same colors.
        Console.ResetColor();
    }

    private string GetLogTypeIcon(LogType logType)
    {
        return logType switch
        {
            LogType.Info => "ℹ️",
            LogType.Success => "✅",
            LogType.Warning => "⚠️",
            LogType.Error => "❌",
            LogType.Debug => "🐛",
            _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
        };
    }
}