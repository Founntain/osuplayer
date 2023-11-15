using Avalonia.Controls;
using Avalonia.Interactivity;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using Splat;
using TappedEventArgs = Avalonia.Input.TappedEventArgs;

namespace OsuPlayer.Views;

public partial class HomeView : ReactiveControl<HomeViewModel>
{
    private FluentAppWindow? _mainWindow;

    public HomeView()
    {
        InitializeComponent();

        _mainWindow = Locator.Current.GetRequiredService<FluentAppWindow>();

        HomeViewInitialized();
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

    private void AddToBlacklist_OnClick(object? sender, RoutedEventArgs e)
    {
        using var blacklist = new Blacklist();
        blacklist.Container.Songs.Add(ViewModel.SelectedSong?.Hash);
    }
}