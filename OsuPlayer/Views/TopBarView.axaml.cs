using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Modules.Party;
using OsuPlayer.Windows;
using ReactiveUI;
using Splat;

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
            case "SymmetricalNavigation":
                _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.SymmetricalView;
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
                var partyManager = Locator.Current.GetRequiredService<PartyManager>();
                
                _mainWindow.ViewModel!.MainView = partyManager.CurrentParty.Value == default  ? _mainWindow.ViewModel.PartyListView : _mainWindow.ViewModel.PartyView;
                
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