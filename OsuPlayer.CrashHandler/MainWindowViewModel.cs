using Nein.Base;
using ReactiveUI;

namespace OsuPlayer.CrashHandler;

public class MainWindowViewModel : BaseWindowViewModel
{
    private string _crashLog = string.Empty;

    public string CrashLog
    {
        get => _crashLog;
        set => this.RaiseAndSetIfChanged(ref _crashLog, value);
    }
}