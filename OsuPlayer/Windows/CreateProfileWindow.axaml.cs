﻿using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Windowing;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.User;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Network.Security;
using OsuPlayer.UI_Extensions;
using Splat;

namespace OsuPlayer.Windows;

public partial class CreateProfileWindow : FluentReactiveWindow<CreateProfileWindowViewModel>
{
    public CreateProfileWindow()
    {
        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        var config = new Config();

        TransparencyLevelHint = new[] { WindowTransparencyLevel.Mica, WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.None };
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

        var response = await Locator.Current.GetService<IOsuPlayerApiService>().User.Register(new AddUserModel
        {
            Username = ViewModel.Username,
            Password = ViewModel.Password
        });

        if (response == default)
        {
            await MessageBox.ShowDialogAsync(this,
                "Can't create a profile. Possible reasons could be: The username is already taken or an server error happend. Please try again later or another username!");
            return;
        }

        if (response.GetType() == typeof(UserModel))
        {
            await MessageBox.ShowDialogAsync(this, "Profile created successfully. You can now log in!");
            Close();
            return;
        }

        MessageBox.Show("An error occured on the server side, or the API is not reachable! Please try again later");
    }

    private void OpenTosBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl("https://github.com/Founntain/osuplayer/blob/master/ProfileTOS.md");
    }
}