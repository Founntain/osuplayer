using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Nein.Base;

namespace OsuPlayer.Views;

public partial class PlayHistoryView : ReactiveControl<PlayHistoryViewModel>
{
    public PlayHistoryView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void HistoryListBox_OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        var mapEntryFromHash = ViewModel.SongSourceProvider.GetMapEntryFromHash(ViewModel.SelectedHistoricalMapEntry?.MapEntry.Hash);

        await ViewModel.Player.TryPlaySongAsync(mapEntryFromHash);
    }
}