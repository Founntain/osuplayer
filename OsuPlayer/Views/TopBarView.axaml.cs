using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Nein.Base;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

internal partial class TopBarView : ReactiveControl<TopBarViewModel>
{
    private MainWindow? _mainWindow;

    public TopBarView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(_ =>
        {
            if (this.GetVisualRoot() is MainWindow mainWindow)
                _mainWindow = mainWindow;
        });
        AvaloniaXamlLoader.Load(this);
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