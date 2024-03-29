﻿using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using FluentAvalonia.UI.Windowing;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.DataModels;
using OsuPlayer.Extensions.ValueConverters;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.UI_Extensions;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Windows;

public partial class LoginWindow : FluentReactiveWindow<LoginWindowViewModel>
{
    private readonly IProfileManagerService _profileManager;

    public LoginWindow() :this(Locator.Current.GetRequiredService<IProfileManagerService>(), string.Empty)
    {
    }

    public LoginWindow(IProfileManagerService profileManager, string username)
    {
        Init();

        _profileManager = profileManager;

        if (ViewModel == default) return;

        ViewModel.Username = username;
    }

    private void Init()
    {
        InitializeComponent();

        ViewModel = new LoginWindowViewModel();

        var config = new Config();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        TransparencyLevelHint = new[] { WindowTransparencyLevel.Mica, WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.None };

        this.WhenActivated(_ =>
        {
            if (string.IsNullOrWhiteSpace(ViewModel?.Username))
            {
                UsernameBox.Focus();

                return;
            }

            PasswordBox.Focus();
        });
    }

    private async Task Login()
    {
        if (ViewModel == default) return;

        var user = await Locator.Current.GetService<IOsuPlayerApiService>().LoginAndSaveAuthToken(ViewModel.Username, ViewModel.Password);

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

        _profileManager.User = user.ConvertObjectToJson<User>();

        var mainWindow = Locator.Current.GetRequiredService<FluentAppWindow>();

        mainWindow.LoginNavItem.IsVisible = mainWindow.ViewModel!.HomeView.IsUserNotLoggedIn;
        mainWindow.EditUserNavItem.IsVisible = mainWindow.ViewModel!.HomeView.IsUserLoggedIn;

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