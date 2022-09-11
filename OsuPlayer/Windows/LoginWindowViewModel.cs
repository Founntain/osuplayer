using OsuPlayer.Base.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Windows;

public class LoginWindowViewModel : BaseWindowViewModel
{
    private string _password;
    private string _username;

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