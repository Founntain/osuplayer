using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Windows;

public class CreateProfileWindowViewModel : BaseWindowViewModel
{
    private string _username;
    private string _password;
    private bool _isTosChecked;

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

    public bool IsTosChecked
    {
        get => _isTosChecked;
        set => this.RaiseAndSetIfChanged(ref _isTosChecked, value);
    }
}