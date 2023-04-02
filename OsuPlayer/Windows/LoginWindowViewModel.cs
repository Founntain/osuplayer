using Nein.Base;
using ReactiveUI;

namespace OsuPlayer.Windows;

public class LoginWindowViewModel : BaseWindowViewModel
{
    private string _password = string.Empty;
    private string _username = string.Empty;

    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }
}