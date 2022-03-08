using System.Threading.Tasks;
using OsuPlayer.Modules.Network.API.ApiEndpoints;
using OsuPlayer.Modules.Network.Online;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class LoginWindowViewModel : BaseViewModel
{
    private string _username;
    private string _password;

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

    public async Task Login()
    {
        var user = await ApiAsync.LoadUserWithCredentialsAsync(Username, Password);

        if (user == default) return;

        if (user.Name != Username) return;

        ProfileManager.User = user;
    }
}