using Avalonia;
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
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }

    private void NavigationPressed(object? sender, PointerReleasedEventArgs e)
    {
        switch ((sender as Border)?.Name)
        {
            case "SearchNavigation":
                Core.MainWindow.ViewModel!.MainView = Core.MainWindow.ViewModel.SearchView;
                break;
            case "PlaylistNavigation":
                Core.MainWindow.ViewModel!.MainView = Core.MainWindow.ViewModel.PlaylistView;
                break;
            case "HomeNavigation":
                Core.MainWindow.ViewModel!.MainView = Core.MainWindow.ViewModel.HomeView;
                break;
            case "UserNavigation":
                Core.MainWindow.ViewModel!.MainView = Core.MainWindow.ViewModel.UserView;
                break;
            case "PartyNavigation":
                Core.MainWindow.ViewModel!.MainView = Core.MainWindow.ViewModel.PartyView;
                break;
            default:
                break;
        }
    }

    private void WindowButtonPressed(object? sender, PointerReleasedEventArgs e)
    {
        switch ((sender as Image)?.Name)
        {
            case "Minimize":
                Core.MainWindow.WindowState = WindowState.Minimized;
                break;
            case "Miniplayer":
                //TODO: open mini player
                break;
            case "Close":
                Core.MainWindow.Close();
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
}