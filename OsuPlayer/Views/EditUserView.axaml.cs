using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using Nein.Extensions;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.RequestModels.User;
using OsuPlayer.Data.DataModels;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Network.API.NorthFox;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public partial class EditUserView : ReactiveUserControl<EditUserViewModel>
{
    private FluentAppWindow? _mainWindow;
    private readonly IProfileManagerService _profileManager;

    public EditUserView() : this(Locator.Current.GetService<IProfileManagerService>())
    {
    }

    public EditUserView(IProfileManagerService profileManager)
    {
        _profileManager = profileManager;
        _mainWindow = Locator.Current.GetRequiredService<FluentAppWindow>();

        InitializeComponent();
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

        if (Locator.Current.GetService<IOsuPlayerApiService>() is not NorthFox api) return;

        var serverUser = await api.User.GetUserFromLoginToken();

        if (serverUser == default)
        {
            await MessageBox.ShowDialogAsync(_mainWindow, "Couldn't fetch profile from server! Please try again later");

            return;
        }

        var changedProfilePicture = ViewModel.IsNewProfilePictureSelected;

        if (!await SaveUserInputValidation(serverUser)) return;

        var editUserModel = new EditUserModel
        {
            User = ViewModel.CurrentUser,
            NewUsername = string.IsNullOrWhiteSpace(ViewModel.NewUsername)
                ? null
                : ViewModel.NewUsername,
            NewPassword = string.IsNullOrWhiteSpace(ViewModel.NewPassword)
                ? null
                : ViewModel.NewPassword
        };

        var response = await api.User.EditUser(editUserModel);

        if (response == default)
            return;

        if (ViewModel == null)
        {
            if (!string.IsNullOrWhiteSpace(editUserModel.User.Name))
                _profileManager.User = (await api.User.GetUserFromLoginToken())?.ConvertObjectToJson<User>();

            if (changedProfilePicture)
                await MessageBox.ShowDialogAsync(_mainWindow,
                    $"We couldn't update your profile picture, because you left the edit view to early!{Environment.NewLine}If you want to update your profile picture please wait, until you get the message that it's been done!");
        }
        else
        {
            ViewModel.NewPassword = string.Empty;
            ViewModel.Password = string.Empty;

            _profileManager.User = ViewModel.CurrentUser.ConvertObjectToJson<User>();

            var successMessage = "Profile updated successfully!";

            if (response.Name == ViewModel.NewUsername && serverUser.Name != response.Name)
                successMessage = "Profile and username updated successfully. Restart your client to see you new username!";

            ViewModel.NewUsername = string.Empty;

            await MessageBox.ShowDialogAsync(_mainWindow, successMessage);

            if (changedProfilePicture)
                await UpdateProfilePicture();
        }
    }

    private async Task<bool> SaveUserInputValidation(UserModel serverUser)
    {
        if (_mainWindow == default || ViewModel == default) return false;

        if (string.IsNullOrWhiteSpace(ViewModel.Password))
        {
            await MessageBox.ShowDialogAsync(_mainWindow, "Please enter your current password to save your changes!");

            return false;
        }

        if (ViewModel.CurrentUser?.OsuProfile?.Length > 0 && !ViewModel.CurrentUser.OsuProfile.IsDigitsOnly())
        {
            await MessageBox.ShowDialogAsync(_mainWindow, "Your osu!profile ID should only contain numbers");

            return false;
        }

        if (!string.IsNullOrWhiteSpace(ViewModel.NewUsername) && !Regex.IsMatch(ViewModel.NewUsername, "^[a-zA-Z0-9]*$"))
        {
            await MessageBox.ShowDialogAsync(_mainWindow, "Your newly entered username can only contain Letters and Numbers!");

            return false;
        }

        return true;

    }

    private async Task UpdateProfilePicture()
    {
        if (_mainWindow == default || ViewModel?.CurrentUser == default || ViewModel?.CurrentProfilePicture == default) return;

        if (Locator.Current.GetService<IOsuPlayerApiService>() is not NorthFox api) return;

        await using var stream = new MemoryStream();

        ViewModel.CurrentProfilePicture.Save(stream);

        var response = await api.User.SaveProfilePicture(stream.ToArray());

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

        var banner = await Locator.Current.GetService<IOsuPlayerApiService>().User.GetProfileBannerAsync(ViewModel.CurrentUser.CustomBannerUrl);

        if (banner == default) return;

        ViewModel.CurrentProfileBanner = banner;
    }

    private async void ResetBanner_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.CurrentUser == default || string.IsNullOrWhiteSpace(ViewModel.CurrentUser.Name)) return;

        var api = Locator.Current.GetService<IOsuPlayerApiService>();

        var user = await api.User.GetUserFromLoginToken();

        if (user == default) return;

        ViewModel.CurrentProfileBannerUrl = user.CustomBannerUrl;

        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentProfileBannerUrl));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentUser.CustomBannerUrl));

        if (ViewModel?.CurrentUser == default) return;

        var banner = await api.User.GetProfileBannerAsync(ViewModel.CurrentUser.CustomBannerUrl);

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
        if (_mainWindow?.ViewModel == default || _profileManager.User == default || ViewModel == default) return;

        if (string.IsNullOrWhiteSpace(ViewModel.ConfirmDeletionPassword))
        {
            MessageBox.Show("Please enter your password, to confirm your deletion!");

            return;
        }

        var response = await Locator.Current.GetService<IOsuPlayerApiService>().User.DeleteUser();

        if (!response)
        {
            MessageBox.Show("You are not authorized to delete this profile or an error occured!");

            return;
        }

        _profileManager.User = default;

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