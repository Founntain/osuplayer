using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.Modules.IO;
using OsuPlayer.UI_Extensions;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class HomeView : ReactiveUserControl<HomeViewModel>
{
    public HomeView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables => { HomeViewInitialized(); });
        AvaloniaXamlLoader.Load(this);
    }

    private async void HomeViewInitialized()
    {
        var osuPath = Core.Instance.Config.OsuPath;

        if (string.IsNullOrWhiteSpace(osuPath))
        {
            await MessageBox.ShowDialogAsync(Core.Instance.MainWindow,
                "You have to select your osu!.db file, before you can start listening to your songs.\nPlease head to the settings to select your osu!.db.");
        }

        //ViewModel!.Songs = new ObservableCollection<SongEntry>(songs);
    }

    private void InputElement_OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        var list = sender as ListBox;
        var song = list.SelectedItem as SongEntry;
        Core.Instance.Player.Play(song);
    }
}