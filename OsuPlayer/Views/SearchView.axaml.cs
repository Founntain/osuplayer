using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OsuPlayer.IO.DbReader.DataModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class SearchView : ReactivePlayerControl<SearchViewModel>
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
        var song = list!.SelectedItem as DbMapEntryBase;
        await ViewModel.Player.PlayAsync(song);
    }
}