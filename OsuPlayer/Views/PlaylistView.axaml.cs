using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OsuPlayer.Data.OsuPlayer.Database.Entities;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Database;
using OsuPlayer.ViewModels;

namespace OsuPlayer.Views;

public partial class PlaylistView : UserControl
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
        var vm = (PlaylistViewModel) DataContext!;

        vm.Playlists = (await RealmDatabase.ReadAll<Playlist>()).ToObservableCollection();
    }
}