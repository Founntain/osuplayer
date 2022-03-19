using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class PlaylistEditorView : ReactiveUserControl<PlaylistEditorViewModel>
{
    public PlaylistEditorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void AddToPlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentSelectedPlaylist == default)
        {
            if (ViewModel.Playlists.Count > 0)
            {
                ViewModel.CurrentSelectedPlaylist = ViewModel.Playlists.Items.ElementAt(0);
            }
            else
            {
                return;
            }
        }

        var playlist = ViewModel!.CurrentSelectedPlaylist.Songs;

        foreach (var song in ViewModel.SelectedSonglistItems)
        {
            if (playlist.Contains(song.BeatmapChecksum))
                continue;
            
            playlist.Add(song.BeatmapChecksum);
        }

        var updatePlaylist = new Playlist()
        {
            Name = ViewModel.CurrentSelectedPlaylist.Name,
            Songs = playlist
        };

        ViewModel.CurrentSelectedPlaylist = updatePlaylist;
        
        await PlaylistManager.ReplacePlaylistAsync(ViewModel.CurrentSelectedPlaylist);

        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentSelectedPlaylist));
    }
    
    private async void RemoveFromPlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentSelectedPlaylist == default)
        {
            if (ViewModel.Playlists.Count > 0)
            {
                ViewModel.CurrentSelectedPlaylist = ViewModel.Playlists.Items.ElementAt(0);
            }
            else
            {
                return;
            }
        }

        var playlist = ViewModel!.CurrentSelectedPlaylist.Songs;

        foreach (var song in ViewModel.SelectedPlaylistItems)
        {
            if (!playlist.Contains(song.BeatmapChecksum))
                continue;
            
            playlist.Remove(song.BeatmapChecksum);
        }

        ViewModel.CurrentSelectedPlaylist = new()
        {
            Name = ViewModel.CurrentSelectedPlaylist.Name,
            Songs = playlist
        };
        
        await PlaylistManager.ReplacePlaylistAsync(ViewModel.CurrentSelectedPlaylist);

        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentSelectedPlaylist));
    }

    private void Songlist_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var listBox = (ListBox) sender!;
        
        var vm = (PlaylistEditorViewModel) DataContext;

        if (vm == default) 
            return;

        var songs = listBox.SelectedItems.Cast<MapEntry>().ToList();
        
        vm.SelectedSonglistItems = songs;
    }

    private void Playlist_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var listBox = (ListBox) sender!;
        
        var vm = (PlaylistEditorViewModel) DataContext;
        
        if (vm == default) 
            return;

        var songs = listBox.SelectedItems.Cast<MapEntry>().ToList();
        
        vm.SelectedPlaylistItems = songs;
    }
}