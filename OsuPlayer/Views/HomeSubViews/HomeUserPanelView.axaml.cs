using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views.HomeSubViews;

public partial class HomeUserPanelView : ReactiveUserControl<HomeUserPanelViewModel>
{
    private FluentAppWindow _mainWindow;

    public HomeUserPanelView()
    {
        InitializeComponent();

        this.WhenActivated(_ =>
        {
            if (this.GetVisualRoot() is FluentAppWindow mainWindow)
                _mainWindow = mainWindow;
        });
    }

    private void GoToSettings_Click(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow.ViewModel == default) return;

        _mainWindow.ViewModel.SettingsView.SettingsSearchQ = "Display User Statistics";
        _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.SettingsView;
        _mainWindow.ViewModel.RaisePropertyChanged(nameof(_mainWindow.ViewModel.SettingsView.SettingsSearchQ));
    }
}