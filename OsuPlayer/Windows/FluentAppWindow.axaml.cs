using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.IO.Importer;
using OsuPlayer.Modules;

namespace OsuPlayer.Windows;

public partial class FluentAppWindow : FluentReactiveWindow<FluentAppWindowViewModel>
{
    private readonly ILoggingService _loggingService;
    private readonly IProfileManagerService _profileManager;

    public FluentAppWindow(FluentAppWindowViewModel viewModel, ILoggingService loggingService)
    {
        ViewModel = viewModel;

        var player = ViewModel.Player;

        Task.Run(() => SongImporter.ImportSongsAsync(player.SongSourceProvider, player as IImportNotifications));

        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        TransparencyLevelHint = new [] { WindowTransparencyLevel.Mica, WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.None };

        _profileManager = ViewModel.ProfileManager;
        _loggingService = loggingService;

        ViewModel.MainView = ViewModel.HomeView;
    }

    private void AppNavigationView_OnItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.IsSettingsInvoked)
        {
            ViewModel!.MainView = ViewModel.SettingsView;

            return;
        }

        ViewModel!.MainView = e.InvokedItemContainer.Tag switch
        {
            "BeatmapsNavigation" => ViewModel.BeatmapView,
            "SearchNavigation" => ViewModel.SearchView,
            "PlaylistNavigation" => ViewModel.PlaylistView,
            "HomeNavigation" => ViewModel.HomeView,
            "UserNavigation" => ViewModel.UserView,
            "PartyNavigation" => ViewModel.PartyView,
            "StatisticsNavigation" => ViewModel.StatisticsView,
            _ => ViewModel!.HomeView
        };
    }

    private async void SearchBox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;

        var acb = (sender as AutoCompleteBox);

        if (acb?.SelectedItem is IMapEntryBase map)
        {
            var result = await ViewModel?.Player.TryPlaySongAsync(map);

            if (result)
            {
                acb.Text = null;
                return;
            }
        }

        e.Handled = true;
    }
}