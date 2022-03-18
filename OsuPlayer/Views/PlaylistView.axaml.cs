using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.Data.OsuPlayer.Database.Entities;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Database;
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
        ViewModel!.Playlists = (await RealmDatabase.ReadAll<Playlist>()).ToObservableCollection();
    }
}