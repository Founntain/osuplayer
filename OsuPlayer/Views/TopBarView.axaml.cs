using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

internal partial class TopBarView : ReactivePlayerControl<TopBarViewModel>
{
    private MainWindow _mainWindow;

    public TopBarView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            if (this.GetVisualRoot() is MainWindow mainWindow)
                _mainWindow = mainWindow;
        });
        AvaloniaXamlLoader.Load(this);
    }

    private void Navigation_Clicked(object? sender, RoutedEventArgs e)
    {
        switch ((sender as Control)?.Name)
        {
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
        }
    }

    private void TopBarGrid_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _mainWindow.BeginMoveDrag(e);
        e.Handled = false;
    }
}