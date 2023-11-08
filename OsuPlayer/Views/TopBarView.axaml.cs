using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Windows;
using Splat;

namespace OsuPlayer.Views;

internal partial class TopBarView : ReactiveControl<TopBarViewModel>
{
    private FluentAppWindow? _mainWindow;

    public TopBarView()
    {
        InitializeComponent();

        _mainWindow = Locator.Current.GetRequiredService<FluentAppWindow>();
    }
    private void Navigation_Clicked(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow?.ViewModel == default) return;

        switch ((sender as Control)?.Name)
        {
            case "BeatmapsNavigation":
                _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.BeatmapView;
                break;
            case "SearchNavigation":
                _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.SearchView;
                break;
            case "PlaylistNavigation":
                _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.PlaylistView;
                break;
            case "HomeNavigation":
                _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.HomeView;
                break;
            case "UserNavigation":
                _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.UserView;
                break;
            case "PartyNavigation":
                _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.PartyView;
                break;
            case "StatisticsNavigation":
                _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.StatisticsView;
                break;
        }
    }

    private void TopBarGrid_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_mainWindow == default) return;

        _mainWindow.BeginMoveDrag(e);
        e.Handled = false;
    }
}