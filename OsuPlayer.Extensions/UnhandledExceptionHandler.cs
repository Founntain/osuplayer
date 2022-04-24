namespace OsuPlayer.Extensions;

/// <summary>
/// A class to handle an unhandled <see cref="Exception" /> thrown in the Main method.
/// </summary>
public static class UnhandledExceptionHandler
{
    /// <summary>
    /// Creates missing log directories if it does not exists
    /// </summary>
    private static void CreatLogsDirectoryIfItsMissing()
    {
        Directory.CreateDirectory("logs");
    }

    /// <summary>
    /// Create a crashlog from an unhandled exception and save it inside the logs folder.
    /// </summary>
    /// <param name="ex">The exception that should be logged</param>
    public static void HandleException(Exception ex)
    {
        CreatLogsDirectoryIfItsMissing();

        var dateString = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

        //Not using concatenation so it is better readable
        var crashlog = "A shit, it looks like you did something that the osu!player did not like. If you think this is a bug, please report this crashlog to Founntain via the following methods:"
                       + Environment.NewLine
                       + Environment.NewLine + "🐈 GitHub: https://github.com/Founntain/osuplayer/issues/new?assignees=&labels=bug&template=bug_report.md&title=%5BBUG%5D+"
                       + Environment.NewLine + "🗨️ Discord: https://discord.gg/RJQSc5B"
                       + Environment.NewLine + "✉️ Email: 7@founntain.dev"
                       + Environment.NewLine
                       + Environment.NewLine
                       + "🛑 Error stacktrace below:"
                       + Environment.NewLine
                       + ex.Message + Environment.NewLine + ex.StackTrace;
        
        File.WriteAllText($"logs/{dateString}.txt", crashlog);
    }
}