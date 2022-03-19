using System;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Playlists;
using OsuPlayer.ViewModels;

namespace OsuPlayer.Views;

public partial class PlaylistView : ReactiveUserControl<PlaylistViewModel>
{
    public PlaylistView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);  
    }

    private async void PlaylistView_OnInitialized(object? sender, EventArgs e)
    {
        ViewModel.Playlists = (await PlaylistManager.GetAllPlaylistsAsync()).ToObservableCollection();
    }
}