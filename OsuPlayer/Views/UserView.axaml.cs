using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using OsuPlayer.Data.API.Models.Beatmap;

namespace OsuPlayer.Views;

public partial class UserView : ReactivePlayerControl<UserViewModel>
{
    public UserView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var viewer = this.FindControl<ContentPresenter>("BadgeItems");
        var list = (ListBox) sender;

        if (list?.SelectedItem == default) return;

        var items = ViewModel!.LoadBadges((User) list.SelectedItem);

        viewer.Content = new ItemsRepeater
        {
            Items = items,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
            Layout = new WrapLayout
            {
                Orientation = Orientation.Horizontal
            }
        };

        viewer.UpdateChild();
    }

    private async void InputElement_OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        var listBox = (ListBox) sender;
        if (listBox == default) return;
        var beatmapModel = (BeatmapUserValidityModel) listBox.SelectedItem;
        if (beatmapModel == default || ViewModel.Player.SongSourceList == default) return;
        var mapEntry = ViewModel.Player.SongSourceList.FirstOrDefault(x =>
            x.BeatmapSetId == beatmapModel.Beatmap.BeatmapSetId ||
            (x.Artist == beatmapModel.Beatmap.Artist && x.Title == beatmapModel.Beatmap.Title));
        if (mapEntry != default)
            await ViewModel.Player.TryPlaySongAsync(mapEntry);
    }
}