using Avalonia.Input;
using Nein.Base;

namespace OsuPlayer.Views;

public partial class PlayHistoryView : ReactiveControl<PlayHistoryViewModel>
{
    public PlayHistoryView()
    {
        InitializeComponent();
    }

    private async void HistoryListBox_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var mapEntryFromHash = ViewModel.SongSourceProvider.GetMapEntryFromHash(ViewModel.SelectedHistoricalMapEntry?.MapEntry.Hash);

        await ViewModel.Player.TryPlaySongAsync(mapEntryFromHash);
    }
}