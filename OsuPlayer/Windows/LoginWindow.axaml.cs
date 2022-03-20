using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.Network.API.ApiEndpoints;
using OsuPlayer.Network.Online;

namespace OsuPlayer.Windows;

public partial class LoginWindow : ReactiveWindow<LoginWindowViewModel>
{
    public LoginWindow()
    {
        InitializeComponent();

        using var config = new Config();
        TransparencyLevelHint = config.Read().TransparencyLevelHint;
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void LoginBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == default) return;
        
        var user = await ApiAsync.LoadUserWithCredentialsAsync(ViewModel.Username, ViewModel.Password);

        if (user == default) return;

        if (user.Name != ViewModel.Username) return;

        ProfileManager.User = user;
        
        Close();
    }
}