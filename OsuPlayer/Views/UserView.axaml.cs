using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Api.Data.API.RequestModels.Statistics;
using OsuPlayer.Data.DataModels;

namespace OsuPlayer.Views;

public partial class UserView : ReactiveControl<UserViewModel>
{
    public UserView()
    {
        InitializeComponent();
    }

    private void UserList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // var viewer = this.FindControl<ContentPresenter>("BadgeItems");
        var list = (ListBox) sender;

        if (list?.SelectedItem == default) return;

        var user = list?.SelectedItem.ConvertObjectToJson<User>();

        // var items = ViewModel!.LoadBadges(user);

        // viewer.Content = new ItemsRepeater
        // {
        //     Items = items,
        //     HorizontalAlignment = HorizontalAlignment.Center,
        //     VerticalAlignment = VerticalAlignment.Top,
        //     Layout = new WrapLayout
        //     {
        //         Orientation = Orientation.Horizontal
        //     }
        // };
        //
        // viewer.Margin = new Thickness(0, 0, 0, 5);
        //
        // viewer.UpdateChild();
    }

    private async void UserTopSongsList_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var listBox = (ListBox) sender;

        if (listBox == default) return;

        var beatmapModel = (BeatmapTimesPlayedModel) listBox.SelectedItem;

        if (beatmapModel == default || ViewModel.Player.SongSourceProvider.SongSourceList == default) return;

        var mapEntry = ViewModel.Player.SongSourceProvider.SongSourceList.FirstOrDefault(x =>
            x.BeatmapSetId == beatmapModel.Beatmap.BeatmapSetId ||
            (x.Artist == beatmapModel.Beatmap.Artist && x.Title == beatmapModel.Beatmap.Title));

        if (mapEntry != default)
            await ViewModel.Player.TryPlaySongAsync(mapEntry);
    }

    private void OsuProfile_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedUser == default) return;

        GeneralExtensions.OpenUrl($"https://osu.ppy.sh/users/{ViewModel.SelectedUser.OsuProfile}");
    }
}