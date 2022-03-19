using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.Storage.Playlists;
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
        this.WhenActivated(disposables =>
        {
            if (ViewModel.CurrentSelectedPlaylist == default)
                return;
        });
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

        foreach (var song in ViewModel.SelectedSongListItems)
        {
            if (playlist.Contains(song.BeatmapChecksum))
                continue;
            
            playlist.Add(song.BeatmapChecksum);
        }
        
        ViewModel!.SelectedSongListItems = new List<MapEntry>();
        
        await PlaylistManager.ReplacePlaylistAsync(ViewModel.CurrentSelectedPlaylist);

        ViewModel.RaisePropertyChanged(nameof(ViewModel.Playlists));
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

        foreach (var song in ViewModel!.SelectedPlaylistItems!)
        {
            if (!playlist.Contains(song.BeatmapChecksum))
                continue;
            
            playlist.Remove(song.BeatmapChecksum);
        }

        ViewModel!.SelectedPlaylistItems = new List<MapEntry>();
        
        await PlaylistManager.ReplacePlaylistAsync(ViewModel.CurrentSelectedPlaylist);

        ViewModel.RaisePropertyChanged(nameof(ViewModel.Playlists));
    }

    private void SongList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ViewModel == default) return;
        
        var listBox = (ListBox) sender!;

        var songs = listBox.SelectedItems.Cast<MapEntry>().ToList();
        
        ViewModel.SelectedSongListItems = songs;
    }

    private void Playlist_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ViewModel == default) return;
        
        var listBox = (ListBox) sender!;

        var songs = listBox.SelectedItems.Cast<MapEntry>().ToList();
        
        ViewModel.SelectedPlaylistItems = songs;
    }
}