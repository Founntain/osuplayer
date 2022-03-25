using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.Data.API.Models.User;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.Modules.Security;
using OsuPlayer.Network.API.ApiEndpoints;
using OsuPlayer.UI_Extensions;

namespace OsuPlayer.Windows;

public partial class CreateProfileWindow : ReactiveWindow<CreateProfileWindowViewModel>
{
    public CreateProfileWindow()
    {
        InitializeComponent();
        
        using var config = new Config();
        TransparencyLevelHint = config.Read().TransparencyLevelHint;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void CreateProfile_Click(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == default) return;

        if (string.IsNullOrWhiteSpace(ViewModel.Username) || string.IsNullOrWhiteSpace(ViewModel.Password))
            return;

        if (!await PasswordManager.CheckIfPasswordMeetsRequirements(ViewModel.Password)) return;

        var response = await ApiAsync.ApiRequestAsync<string>("users", "createUser", new CreateUserModel
        {
            UserModel = new UserModel
            {
                Name = ViewModel.Username
            },
            Password = ViewModel.Password
        });

        switch (response)
        {
            case "User already exists":
                await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, "User already exists, please try another username!");
                return;
            case "Profile creation failed":
                await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, "Profile couldn't be created!");

                return;
        }

        if (!string.IsNullOrWhiteSpace(response)) return;

        await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, "And error occured on the server side, or the API is not reachable! Please try again later");
    }

    private void OpenTosBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl("https://github.com/Founntain/osuplayer/blob/master/ProfileTOS.md");
    }
}