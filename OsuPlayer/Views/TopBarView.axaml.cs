using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

partial class TopBarView : ReactiveUserControl<TopBarViewModel>
{
    public TopBarView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            
        });
        AvaloniaXamlLoader.Load(this);
    }

    private void NavigationPressed(object? sender, PointerReleasedEventArgs e)
    {
        switch ((sender as Control)?.Name)
        {
            case "SearchNavigation":
                Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.SearchView;
                break;
            case "PlaylistNavigation":
                Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.PlaylistView;
                break;
            case "HomeNavigation":
                Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.HomeView;
                break;
            case "UserNavigation":
                Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.UserView;
                break;
            case "PartyNavigation":
                Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.PartyView;
                break;
            default:
                break;
        }
    }

    private void WindowButtonPressed(object? sender, PointerReleasedEventArgs e)
    {
        switch ((sender as Control)?.Name)
        {
            case "Minimize":
                Core.Instance.MainWindow.WindowState = WindowState.Minimized;
                break;
            case "Miniplayer":
                //TODO: open mini player
                break;
            case "Close":
                Core.Instance.MainWindow.Close();
                break;
        }
    }

    private void OpenFullscreenMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void OpenMiniplayerMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void OpenEqualizerMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void OpenContributersMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
    
    private void TopBarGrid_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Core.Instance.MainWindow.BeginMoveDrag(e);
        e.Handled = false;
    }
}