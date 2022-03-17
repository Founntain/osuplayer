using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DynamicData;
using OsuPlayer.IO.Database;
using OsuPlayer.IO.Database.Entities;
using OsuPlayer.IO.DbReader;
using OsuPlayer.ViewModels;

namespace OsuPlayer.Views;

public partial class PlaylistEditorView : UserControl
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
        await using var ctx = new DatabaseContext();
        
        if (ctx.Songs.Any(x => x.Songchecksum == checksum))
            return;

        var song = new Song
        {
            Songchecksum = checksum
        };
        
        await ctx.Songs.AddAsync(song);

        await ctx.SaveChangesAsync();
    }

    private async void AddToPlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        var vm = (PlaylistEditorViewModel) DataContext!;

        var playlist = vm.Playlist;

        foreach (var song in vm.SelectedSonglistItems)
        {
            if (playlist.Contains(song))
                continue;
            
            playlist.Add(song);

            await AddSongChecksumToDatabaseIfNotExists(song.BeatmapChecksum);
        }

        vm.Playlist = new ObservableCollection<MapEntry>(playlist);
    }
    
    private async void RemoveFromPlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        var vm = (PlaylistEditorViewModel) DataContext!;

        var playlist = vm.Playlist;

        playlist.RemoveMany(vm.SelectedPlaylistItems);

        vm.Playlist = new ObservableCollection<MapEntry>(playlist);
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