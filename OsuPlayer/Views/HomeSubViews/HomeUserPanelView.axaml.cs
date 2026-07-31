using Avalonia.Interactivity;
using Nein.Extensions;
using OsuPlayer.Windows;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Splat;

namespace OsuPlayer.Views.HomeSubViews;

public partial class HomeUserPanelView : ReactiveUserControl<HomeUserPanelViewModel>
{
    private FluentAppWindow _mainWindow;

    public HomeUserPanelView()
    {
        InitializeComponent();

        this.WhenActivated((Action<IDisposable> _) =>
        {
            _mainWindow = Locator.Current.GetRequiredService<FluentAppWindow>();
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