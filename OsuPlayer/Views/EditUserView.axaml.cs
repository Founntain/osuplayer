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
using Avalonia.VisualTree;
using OsuPlayer.Data.API.Enums;
using OsuPlayer.Data.API.Models.User;
using OsuPlayer.Extensions;
using OsuPlayer.Network.API.ApiEndpoints;
using OsuPlayer.Network.Online;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class EditUserView : ReactiveUserControl<EditUserViewModel>
{
    private MainWindow _mainWindow;

    public EditUserView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            if (this.GetVisualRoot() is MainWindow mainWindow)
                _mainWindow = mainWindow;
        });
        AvaloniaXamlLoader.Load(this);
    }

    private async void EditProfilePicture_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.CurrentUser == default) return;

        var dialog = new OpenFileDialog();

        dialog.AllowMultiple = false;
        dialog.Filters = new List<FileDialogFilter>
        {
            new()
            {
                Extensions = new List<string>
                {
                    "png",
                    "jpg",
                    "jpeg"
                }
            }
        };

        var result = await dialog.ShowAsync(_mainWindow);

        var file = result?.FirstOrDefault();

        if (file == default) return;

        var fileInfo = new FileInfo(file);

        switch (ViewModel.CurrentUser.IsDonator)
        {
            case false when fileInfo.Length / 1024 / 1024 >= 2:
                await MessageBox.ShowDialogAsync(_mainWindow,
                    "The file you selected is bigger than 2 MB.\n\nIf you want to upload profile pictures up to 4 MB consider getting donator");
                return;
            case true when fileInfo.Length / 1024 / 1024 >= 4:
                await MessageBox.ShowDialogAsync(_mainWindow,
                    "The file you selected is bigger than 4 MB! Sorry :'(");
                return;
        }

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
                Extensions = new List<string>
                {
                    "png",
                    "jpg",
                    "jpeg"
                }
            }
        };

        var result = await dialog.ShowAsync(_mainWindow);

        var file = result?.FirstOrDefault();

        if (file == default) return;

        var fileInfo = new FileInfo(file);

        var banner = await File.ReadAllBytesAsync(file);

        await using (var stream = new MemoryStream(banner))
        {
            ViewModel.CurrentProfileBanner = new Bitmap(stream);
        }

        ViewModel.IsNewBannerSelected = true;
    }

    private void DeleteProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == default) return;

        ViewModel.IsDeleteProfilePopupOpen = !ViewModel.IsDeleteProfilePopupOpen;
    }

    private async void SaveChanges_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.CurrentUser == default) return;

        var tempUser = await ApiAsync.GetProfileByNameAsync(ViewModel.CurrentUser.Name);

        if (tempUser == default)
        {
            await MessageBox.ShowDialogAsync(_mainWindow,
                "Couldn't fetch profile from server! Please try again later");

            return;
        }

        if (string.IsNullOrWhiteSpace(ViewModel.Password))
        {
            await MessageBox.ShowDialogAsync(_mainWindow,
                "Please enter your current password to save your changes!");

            return;
        }

        if (ViewModel.CurrentUser.OsuProfile.Length > 0 && !ViewModel.CurrentUser.OsuProfile.IsDigitsOnly())
        {
            await MessageBox.ShowDialogAsync(_mainWindow,
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
                await MessageBox.ShowDialogAsync(_mainWindow, "Profile edited successfully!");

                break;
            case UserResponse.UserEditedAndPasswordChanged:
                await MessageBox.ShowDialogAsync(_mainWindow,
                    "Profile edited successfully and password changed.");

                break;
            case UserResponse.UserEditedAndPasswordNotChanged:
                await MessageBox.ShowDialogAsync(_mainWindow,
                    "Profile edited successfully. However your password couldn't be changed!");

                break;
            case UserResponse.UserAlreadyExists:
                await MessageBox.ShowDialogAsync(_mainWindow,
                    "Profile not updated because the user already exists. Did you try changing your name?");

                return;
            case UserResponse.PasswordIncorrect:
                await MessageBox.ShowDialogAsync(_mainWindow,
                    "The password you entered is not correct, therefor your profile was not updated!");
                return;

            default:
                await MessageBox.ShowDialogAsync(_mainWindow,
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
                await MessageBox.ShowDialogAsync(_mainWindow, "Profile picture could not be saved!");

                ViewModel.LoadProfilePicture();

                return;
            }

            await MessageBox.ShowDialogAsync(_mainWindow, "Profile picture changed succesfully!");

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

        var user = await ApiAsync.GetProfileByNameAsync(ViewModel.CurrentUser.Name);

        if (user == default) return;

        ViewModel.CurrentProfileBannerUrl = user.CustomWebBackground;

        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentProfileBannerUrl));
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

    private async void ConfirmDeleteProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ProfileManager.User == default || ViewModel == default) return;

        if (string.IsNullOrWhiteSpace(ViewModel.ConfirmDeletionPassword))
        {
            await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, "Please enter your password, to confirm your deletion!");

            return;
        }

        var response = await ApiAsync.ApiRequestAsync<UserResponse>("users", "deleteUser", new
        {
            Id = ProfileManager.User.Id.ToString(),
            Password = ViewModel.ConfirmDeletionPassword
        });

        if (response != UserResponse.UserDeleted)
        {
            if (response == UserResponse.PasswordIncorrect)
            {
                await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, "Profile could not be deleted, because you entered the wrong password!");

                return;
            }
            
            await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, "Profile could not be deleted, due to an server error!");

            return;
        }

        ProfileManager.User = default;

        await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, "Profile deleted!\n\nSee you next time!");

        Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.HomeView;
    }
}