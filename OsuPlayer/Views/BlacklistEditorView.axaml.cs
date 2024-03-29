﻿using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.Windows;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public partial class BlacklistEditorView : ReactiveControl<BlacklistEditorViewModel>
{
    private FluentAppWindow? _mainWindow;

    public BlacklistEditorView()
    {
        InitializeComponent();

        _mainWindow = Locator.Current.GetRequiredService<FluentAppWindow>();
    }

    private async void PlaySong(object? sender, TappedEventArgs e)
    {
        var tapped = (TappedEventArgs) e;
        var controlSource = (Control) tapped.Pointer.Captured;

        if (controlSource?.DataContext is IMapEntryBase song)
            await ViewModel.Player.TryPlaySongAsync(song);
    }

    private async void AddToBlacklist_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedSongListItems == default) return;

        foreach (var song in ViewModel.SelectedSongListItems)
        {
            if (ViewModel.Blacklist?.Songs.Contains(song.Hash) == true)
                continue;

            ViewModel.Blacklist?.Songs.Add(song.Hash);
        }

        await using (var blacklist = new Blacklist())
        {
            blacklist.Container.Songs = ViewModel.Blacklist?.Songs ?? new HashSet<string>();
            ViewModel.Blacklist = blacklist.Container;
        }

        ViewModel.SelectedSongListItems.Clear();
        ViewModel.RaisePropertyChanged(nameof(ViewModel.SelectedSongListItems));
    }

    private async void RemoveFromBlacklist_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedBlacklistItems == null) return;

        foreach (var song in ViewModel.SelectedBlacklistItems)
        {
            if (!ViewModel.Blacklist?.Songs.Contains(song.Hash) == true)
                continue;

            ViewModel.Blacklist?.Songs.Remove(song.Hash);
        }

        await using (var blacklist = new Blacklist())
        {
            blacklist.Container.Songs = ViewModel.Blacklist?.Songs ?? new HashSet<string>();
            ViewModel.Blacklist = blacklist.Container;
        }

        ViewModel.SelectedBlacklistItems.Clear();
    }

    private void SongList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox) return;

        var songs = listBox.SelectedItems.Cast<IMapEntryBase>().ToList();

        if (_mainWindow == default || _mainWindow.ViewModel == default) return;

        _mainWindow.ViewModel.BlacklistEditorView.SelectedSongListItems = songs;
    }

    private void Blacklist_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox) return;

        var songs = listBox.SelectedItems.Cast<IMapEntryBase>().ToList();

        if (_mainWindow == default || _mainWindow.ViewModel == default) return;

        _mainWindow.ViewModel.BlacklistEditorView.SelectedBlacklistItems = songs;
    }
}