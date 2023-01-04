﻿using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using Material.Icons;
using OsuPlayer.Api.Data.API.RequestModels.User;
using OsuPlayer.Extensions;
using OsuPlayer.Network.API.Service.Endpoints;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public partial class EditUserView : ReactiveUserControl<EditUserViewModel>
{
    private MainWindow? _mainWindow;

    public EditUserView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(_ =>
        {
            if (this.GetVisualRoot() is MainWindow mainWindow)
                _mainWindow = mainWindow;
        });

        AvaloniaXamlLoader.Load(this);
    }

    private async void EditProfilePicture_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow == default || ViewModel?.CurrentUser == default) return;

        var dialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
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
        if (_mainWindow == default || ViewModel == default) return;

        var dialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
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
            }
        };

        var result = await dialog.ShowAsync(_mainWindow);

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
        if (ViewModel == default) return;

        ViewModel.IsDeleteProfilePopupOpen = !ViewModel.IsDeleteProfilePopupOpen;
    }

    private async void SaveChanges_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow == default || ViewModel?.CurrentUser == default || string.IsNullOrWhiteSpace(ViewModel?.CurrentUser.Name)) return;

        var api = Locator.Current.GetService<NorthFox>();
        
        var tempUser = await api.GetUserFromLoginToken();
        
        var changedProfilePicture = ViewModel.IsNewProfilePictureSelected;

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

        if (ViewModel.CurrentUser.OsuProfile?.Length > 0 && !ViewModel.CurrentUser.OsuProfile.IsDigitsOnly())
        {
            await MessageBox.ShowDialogAsync(_mainWindow,
                "Your osu!profile ID should only contain numbers");

            return;
        }

        var editUserModel = new EditUserModel
        {
            User = ViewModel.CurrentUser,
            NewPassword = string.IsNullOrWhiteSpace(ViewModel.NewPassword)
                ? null
                : ViewModel.NewPassword
        };

        var response = await api.EditUser(editUserModel);

        if (response == default)
            return;

        if (ViewModel == null)
        {
            if (!string.IsNullOrWhiteSpace(editUserModel.User.Name))
                ProfileManager.User = (await api.GetUserFromLoginToken())?.ConvertObject<User>();

            if (changedProfilePicture)
                await MessageBox.ShowDialogAsync(_mainWindow, $"We couldn't update your profile picture, because you left the edit view to early!{Environment.NewLine}If you want to update your profile picture please wait, until you get the message that it's been done!");
        }
        else
        {
            ViewModel.NewPassword = string.Empty;
            ViewModel.Password = string.Empty;

            ProfileManager.User = ViewModel.CurrentUser.ConvertObject<User>();

            if (changedProfilePicture)
                await UpdateProfilePicture();
        }
    }

    private async Task UpdateProfilePicture()
    {
        if (_mainWindow == default || ViewModel?.CurrentUser == default || ViewModel?.CurrentProfilePicture == default) return;

        var api = Locator.Current.GetService<NorthFox>();
        
        await using var stream = new MemoryStream();

        ViewModel.CurrentProfilePicture.Save(stream);

        var response = await api.SaveProfilePicture(stream.ToArray());

        if (!response)
        {
            await MessageBox.ShowDialogAsync(_mainWindow, "Profile picture could not be saved!");

            ViewModel.LoadProfilePicture();

            return;
        }

        await MessageBox.ShowDialogAsync(_mainWindow, "Profile picture changed succesfully!");

        ViewModel.IsNewProfilePictureSelected = false;
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

        var banner = await Locator.Current.GetService<NorthFox>().GetProfileBannerAsync(ViewModel.CurrentUser.CustomBannerUrl);

        if (banner == default) return;

        ViewModel.CurrentProfileBanner = banner;
    }

    private async void ResetBanner_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.CurrentUser == default || string.IsNullOrWhiteSpace(ViewModel.CurrentUser.Name)) return;

        var api = Locator.Current.GetService<NorthFox>();
        
        var user = await api.GetUserFromLoginToken();

        if (user == default) return;

        ViewModel.CurrentProfileBannerUrl = user.CustomBannerUrl;

        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentProfileBannerUrl));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentUser.CustomBannerUrl));

        if (ViewModel?.CurrentUser == default) return;

        var banner = await api.GetProfileBannerAsync(ViewModel.CurrentUser.CustomBannerUrl);

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
        if (_mainWindow?.ViewModel == default || ProfileManager.User == default || ViewModel == default) return;

        if (string.IsNullOrWhiteSpace(ViewModel.ConfirmDeletionPassword))
        {
            MessageBox.Show("Please enter your password, to confirm your deletion!");

            return;
        }

        var response = await Locator.Current.GetService<NorthFox>().DeleteUser();

        if (!response)
        {
            MessageBox.Show("You are not authorized to delete this profile or an error occured!");

            return;
        }

        ProfileManager.User = default;

        await MessageBox.ShowDialogAsync(_mainWindow, "Profile deleted!\n\nSee you next time!");

        ViewModel.ConfirmDeletionPassword = string.Empty;
        ViewModel.IsDeleteProfilePopupOpen = false;

        _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.HomeView;
    }

    private void GetDonatorPerksBtn_Click(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl("https://github.com/Founntain/osuplayer/wiki/Support-osu!player");
    }
}