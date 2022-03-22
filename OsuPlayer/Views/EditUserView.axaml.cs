using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using OsuPlayer.Data.API.Enums;
using OsuPlayer.Data.API.Models.User;
using OsuPlayer.Extensions;
using OsuPlayer.Network.API.ApiEndpoints;
using OsuPlayer.Network.Online;
using OsuPlayer.UI_Extensions;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class EditUserView : ReactiveUserControl<EditUserViewModel>
{
    public EditUserView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void EditProfilePicture_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == default) return;

        var dialog = new OpenFileDialog();

        dialog.AllowMultiple = false;
        dialog.Filters = new List<FileDialogFilter>
        {
            new()
            {
                Extensions = new List<string> {"png", "jpg", "jpeg"}
            }
        };

        var result = await dialog.ShowAsync(Core.Instance.MainWindow);

        var file = result?.FirstOrDefault();

        if (file == default) return;

        var picture = await File.ReadAllBytesAsync(file);

        await using (var stream = new MemoryStream(picture))
        {
            ViewModel.CurrentProfilePicture = new Bitmap(stream);
        }

        ViewModel.IsNewProfilePictureSelected = true;
    }

    private async void EditBannerPicture_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == default) return;

        var dialog = new OpenFileDialog();

        dialog.AllowMultiple = false;
        dialog.Filters = new List<FileDialogFilter>
        {
            new()
            {
                Extensions = new List<string> {"png", "jpg", "jpeg"}
            }
        };

        var result = await dialog.ShowAsync(Core.Instance.MainWindow);

        var file = result?.FirstOrDefault();

        if (file == default) return;

        var banner = await File.ReadAllBytesAsync(file);

        await using (var stream = new MemoryStream(banner))
        {
            ViewModel.CurrentProfileBanner = new Bitmap(stream);
        }

        ViewModel.IsNewBannerSelected = true;
    }

    private void DeleteProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void SaveChanges_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.CurrentUser == default) return;

        var tempUser = await ApiAsync.GetProfileByNameAsync(ViewModel.CurrentUser.Name);

        if (tempUser == default)
        {
            await MessageBox.ShowDialogAsync(Core.Instance.MainWindow,
                "Couldn't fetch profile from server! Please try again later");

            return;
        }

        if (string.IsNullOrWhiteSpace(ViewModel.Password))
        {
            await MessageBox.ShowDialogAsync(Core.Instance.MainWindow,
                "Please enter your current password to save your changes!");

            return;
        }

        if (ViewModel.CurrentUser.OsuProfile.Length > 0 && !ViewModel.CurrentUser.OsuProfile.IsDigitsOnly())
        {
            await MessageBox.ShowDialogAsync(Core.Instance.MainWindow,
                "Your osu!profile ID should only contain numbers");

            return;
        }

        var editUserModel = new EditUserModel
        {
            UserModel = ViewModel.CurrentUser,
            Password = ViewModel.Password,
            NewPassword = string.IsNullOrWhiteSpace(ViewModel.NewPassword)
                ? null
                : ViewModel.NewPassword
        };

        var response = await ApiAsync.ApiRequestAsync<UserResponse>("users", "editUser", editUserModel);

        ViewModel.NewPassword = string.Empty;
        ViewModel.Password = string.Empty;

        switch (response)
        {
            case UserResponse.UserEdited:
                await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, "Profile edited successfully!");

                break;
            case UserResponse.UserEditedAndPasswordChanged:
                await MessageBox.ShowDialogAsync(Core.Instance.MainWindow,
                    "Profile edited successfully and password changed.");

                break;
            case UserResponse.UserEditedAndPasswordNotChanged:
                await MessageBox.ShowDialogAsync(Core.Instance.MainWindow,
                    "Profile edited successfully. However your password couldn't be changed!");

                break;
            case UserResponse.UserAlreadyExists:
                await MessageBox.ShowDialogAsync(Core.Instance.MainWindow,
                    "Profile not updated because the user already exists. Did you try changing your name?");

                return;
            case UserResponse.PasswordIncorrect:
                await MessageBox.ShowDialogAsync(Core.Instance.MainWindow,
                    "The password you entered is not correct, therefor your profile was not updated!");
                return;

            default:
                await MessageBox.ShowDialogAsync(Core.Instance.MainWindow,
                    string.Format($"We had trouble updating your profile error message:\n\n{response:G}"));

                return;
        }

        ProfileManager.User = ViewModel.CurrentUser;

        if (ViewModel.IsNewProfilePictureSelected)
            await UpdateProfilePicture();
    }

    private async Task UpdateProfilePicture()
    {
        if (ViewModel?.CurrentUser == default || ViewModel?.CurrentProfilePicture == default) return;

        await using (var stream = new MemoryStream())
        {
            ViewModel.CurrentProfilePicture.Save(stream);

            var profilePicture = Convert.ToBase64String(stream.ToArray());

            var response = await ApiAsync.ApiRequestAsync<UserResponse>("users", "saveProfilePicture", new
            {
                ViewModel.CurrentUser.Name,
                Picture = profilePicture
            });

            if (response == UserResponse.CantSaveProfilePicture)
            {
                await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, "Profile picture could not be saved!");

                ViewModel.LoadProfilePicture();

                return;
            }

            await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, "Profile picture changed succesfully!");

            ViewModel.IsNewProfilePictureSelected = false;
        }
    }

    private void ResetBannerPicture_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == default) return;

        ViewModel.LoadProfileBanner();
        ViewModel.IsNewBannerSelected = false;
    }

    private void ResetProfilePicture_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == default) return;

        ViewModel.LoadProfilePicture();
        ViewModel.IsNewProfilePictureSelected = false;
    }

    private async void PreviewBanner_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.CurrentUser == default) return;

        var banner = await ApiAsync.GetProfileBannerAsync(ViewModel.CurrentUser.CustomWebBackground);

        if (banner == default) return;

        ViewModel.CurrentProfileBanner = banner;
    }

    private async void ResetBanner_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.CurrentUser == default) return;

        var user = await ApiAsync.GetUserByName(ViewModel.CurrentUser.Name);

        if (user == default) return;

        ViewModel.CurrentUser.CustomWebBackground = user.CustomWebBackground;

        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentUser));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentUser.CustomWebBackground));

        if (ViewModel?.CurrentUser == default) return;

        var banner = await ApiAsync.GetProfileBannerAsync(ViewModel.CurrentUser.CustomWebBackground);

        if (banner == default)
        {
            ViewModel.CurrentProfileBanner = default;
            return;
        }

        ViewModel.CurrentProfileBanner = banner;
        ViewModel.IsNewBannerSelected = false;
    }
}