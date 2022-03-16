using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.IO;
using OsuPlayer.IO.DbReader;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class SearchView : ReactiveUserControl<SearchViewModel>
{
    public SearchView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }

    private async void InputElement_OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        var list = sender as ListBox;
        var song = list!.SelectedItem as MapEntry;
        await Core.Instance.Player.Play(song);
    }
}