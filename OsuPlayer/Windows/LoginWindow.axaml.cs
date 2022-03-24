using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.Network.API.ApiEndpoints;
using OsuPlayer.Network.Online;
using ReactiveUI;

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
        this.WhenActivated(disposables => { });

        AvaloniaXamlLoader.Load(this);
    }

    private async Task Login()
    {
        if (ViewModel == default) return;

        var user = await ApiAsync.LoadUserWithCredentialsAsync(ViewModel.Username, ViewModel.Password);

        if (user == default) return;

        if (user.Name != ViewModel.Username) return;

        ProfileManager.User = user;

        Close();
    }

    private async void LoginBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        await Login();
    }

    private void Visual_OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        var control = (Control) sender;

        control?.Focus();
    }

    private async void InputElement_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return)
            await Login();
    }

    private async void CreateProfileBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var createProfileWindow = new CreateProfileWindow
        {
            ViewModel = new()
        };

        await createProfileWindow.ShowDialog(this);
    }
}