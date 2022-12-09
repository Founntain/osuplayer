using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.Extensions;
using OsuPlayer.Modules.Audio.Interfaces;

namespace OsuPlayer.Windows;

public partial class Miniplayer : ReactiveWindow<MiniplayerViewModel>
{
    private readonly MainWindow? _mainWindow;

    public Miniplayer()
    {
        InitializeComponent();

        LoadSettings();

#if DEBUG
        this.AttachDevTools();
#endif
    }

    public Miniplayer(MainWindow mainWindow, IPlayer player, IAudioEngine engine)
    {
        InitializeComponent();

        _mainWindow = mainWindow;

        DataContext = new MiniplayerViewModel(player, engine);

        LoadSettings();

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void LoadSettings()
    {
        using var config = new Config();

        Background = new SolidColorBrush(config.Container.BackgroundColor.ToColor());
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void SongControl(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == default) return;
        
        switch ((sender as Control)?.Name)
        {
            case "Repeat":
                ViewModel.Player.RepeatMode.Value = ViewModel.Player.RepeatMode.Value.Next();
                break;
            case "Previous":
                ViewModel.Player.NextSong(PlayDirection.Backwards);
                break;
            case "PlayPause":
                ViewModel.Player.PlayPause();
                break;
            case "Next":
                ViewModel.Player.NextSong(PlayDirection.Forward);
                break;
            case "Shuffle":
                ViewModel.IsShuffle = !ViewModel.IsShuffle;
                break;
        }
    }

    private void RepeatContextMenu_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (ViewModel == default) return;
        
        ViewModel.Player.SelectedPlaylist.Value = (Playlist) (sender as ContextMenu)?.SelectedItem;
    }

    private void TopBarGrid_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
        e.Handled = false;
    }

    private void Volume_OnClick(object? sender, RoutedEventArgs e)
    {
        if(ViewModel == default) return;

        ViewModel.Player.ToggleMute();
    }

    private void PlaybackSpeed_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel!.PlaybackSpeed = 0;
    }

    private void Miniplayer_OnClosed(object? sender, EventArgs e)
    {
        if (_mainWindow == default) return;
        
        _mainWindow.Miniplayer = null;

        _mainWindow.WindowState = WindowState.Normal;
    }
}