using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.Data.API.Models.User;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.Network.API.ApiEndpoints;
using OsuPlayer.Network.Online;
using OsuPlayer.Network.Security;
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
        {
            await MessageBox.ShowDialogAsync(this, "Please enter a username and a password!");
            
            return;
        }

        var passwordRequirements = PasswordManager.CheckIfPasswordMeetsRequirementsWithErrorList(ViewModel.Password);
        
        if (!passwordRequirements.Item1)
        {
            await MessageBox.ShowDialogAsync(this, passwordRequirements.Item2);
            
            return;
        }

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
                MessageBox.Show("User already exists, please try another username!");
                return;
            case "Profile creation failed":
                MessageBox.Show("Profile couldn't be created!");

                return;
        }

        if (!string.IsNullOrWhiteSpace(response))
        {
            await MessageBox.ShowDialogAsync(this, "Profile created successfully. You can now log in!");
            Close();
            return;
        }

        MessageBox.Show("And error occured on the server side, or the API is not reachable! Please try again later");
    }

    private void OpenTosBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl("https://github.com/Founntain/osuplayer/blob/master/ProfileTOS.md");
    }
}