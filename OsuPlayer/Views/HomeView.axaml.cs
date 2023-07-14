using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Nein.Base;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class HomeView : ReactiveControl<HomeViewModel>
{
    private MainWindow? _mainWindow;

    public HomeView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(_ =>
        {
            if (this.GetVisualRoot() is MainWindow mainWindow)
                _mainWindow = mainWindow;

            HomeViewInitialized();
        });

        AvaloniaXamlLoader.Load(this);
    }

    private async void HomeViewInitialized()
    {
        if (_mainWindow == default) return;

        var config = new Config();
        var osuPath = (await config.ReadAsync()).OsuPath;

        if (string.IsNullOrWhiteSpace(osuPath))
            await MessageBox.ShowDialogAsync(_mainWindow,
                "Before you can start listening to your songs, you have to import them.\nPlease head to the settings to select your osu!.db or client.realm.");

        //ViewModel!.Songs = new ObservableCollection<SongEntry>(songs);
    }

    private async void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var list = sender as ListBox;
        var song = list!.SelectedItem as IMapEntryBase;

        await ViewModel.Player.TryPlaySongAsync(song);
    }

    private async void LoginBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == default) return;

        var loginWindow = new LoginWindow();

        await loginWindow.ShowDialog(_mainWindow);

        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentUser));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.IsUserLoggedIn));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.IsUserNotLoggedIn));

        ViewModel.ProfilePicture = await ViewModel.LoadProfilePicture();
    }

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow?.ViewModel == default) return;

        _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.EditUserView;
    }

    private void AddToBlacklist_OnClick(object? sender, RoutedEventArgs e)
    {
        using var blacklist = new Blacklist();
        blacklist.Container.Songs.Add(ViewModel.SelectedSong?.Hash);
    }
}