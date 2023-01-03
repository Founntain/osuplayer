using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using OsuPlayer.Extensions;
using OsuPlayer.Network.API.Service.Endpoints;
using OsuPlayer.UI_Extensions;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Windows;

public partial class LoginWindow : ReactiveWindow<LoginWindowViewModel>
{
    public LoginWindow()
    {
        Init();
        
        using var config = new Config();
        
        FontFamily = config.Container.Font ?? FontManager.Current.DefaultFontFamilyName;
    }

    public LoginWindow(string username)
    {
        Init();

        if (ViewModel == default) return;

        ViewModel.Username = username;
    }

    private void Init()
    {
        InitializeComponent();

        ViewModel = new LoginWindowViewModel();

        var config = new Config();
        TransparencyLevelHint = config.Container.TransparencyLevelHint;
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        this.WhenActivated(_ =>
        {
            if (string.IsNullOrWhiteSpace(ViewModel?.Username)) return;

            this.FindControl<TextBox>("PasswordBox").Focus();
        });

        AvaloniaXamlLoader.Load(this);
    }

    private async Task Login()
    {
        if (ViewModel == default) return;

        var user = await Locator.Current.GetService<NorthFox>().LoginAndSaveAuthToken(ViewModel.Username, ViewModel.Password);

        if (user == default)
        {
            await MessageBox.ShowDialogAsync(this, "Login failed, please try again!", "Login failed");

            ViewModel.Password = string.Empty;

            return;
        }

        if (user.Name != ViewModel.Username)
        {
            await MessageBox.ShowDialogAsync(this, "Could not retrieve the correct user from the API! Please try again later!", "Login failed");

            ViewModel.Password = string.Empty;

            return;
        }

        ProfileManager.User = user.ConvertObject<User>();

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
            ViewModel = new CreateProfileWindowViewModel()
        };

        await createProfileWindow.ShowDialog(this);
    }
}