using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Nein.Base;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.IO.Storage.Blacklist;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class SearchView : ReactiveControl<SearchViewModel>
{
    public SearchView()
    {
        InitializeComponent();
    }

    private async void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var list = sender as ListBox;
        var song = list!.SelectedItem as IMapEntryBase;

        await ViewModel.Player.TryPlaySongAsync(song);
    }

    private void AddToBlacklist_OnClick(object? sender, RoutedEventArgs e)
    {
        using var blacklist = new Blacklist();

        blacklist.Container.Songs.Add(ViewModel.SelectedSong?.Hash);
    }
}