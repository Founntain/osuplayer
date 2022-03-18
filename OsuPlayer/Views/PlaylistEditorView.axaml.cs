using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DynamicData;
using OsuPlayer.Data.OsuPlayer.Database.Entities;
using OsuPlayer.IO.Database;
using OsuPlayer.IO.DbReader;
using OsuPlayer.ViewModels;

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

    private async Task AddSongChecksumToDatabaseIfNotExists(string checksum)
    {
        //TODO ADD TO REALM
    }

    private async void AddToPlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        var playlist = ViewModel!.Playlist;

        foreach (var song in ViewModel!.SelectedSonglistItems)
        {
            if (playlist.Contains(song))
                continue;
            
            playlist.Add(song);

            await AddSongChecksumToDatabaseIfNotExists(song.BeatmapChecksum);
        }

        ViewModel!.Playlist = new ObservableCollection<MapEntry>(playlist);
    }
    
    private async void RemoveFromPlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        var playlist = ViewModel!.Playlist;

        playlist.RemoveMany(ViewModel.SelectedPlaylistItems);

        ViewModel.Playlist = new ObservableCollection<MapEntry>(playlist);
    }

    private void Songlist_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var listBox = (ListBox) sender!;
        
        var vm = (PlaylistEditorViewModel) DataContext;

        if (vm == default) 
            return;

        var songs = listBox.SelectedItems.Cast<MapEntry>().ToList();
        
        ViewModel!.SelectedSonglistItems = songs;
    }

    private void Playlist_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var listBox = (ListBox) sender!;

        var songs = listBox.SelectedItems.Cast<MapEntry>().ToList();
        
        ViewModel!.SelectedPlaylistItems = songs;
    }
}