using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views.HomeSubViews;

public partial class HomeUserPanelView : ReactiveUserControl<HomeUserPanelViewModel>
{
    private MainWindow _mainWindow;
    
    public HomeUserPanelView()
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

    private async void LoginBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == default) return;

        var loginWindow = new LoginWindow();

        await loginWindow.ShowDialog(_mainWindow);

        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentUser));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.IsUserLoggedIn));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.IsUserNotLoggedIn));

        await ViewModel.LoadUserProfileAsync();
    }

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow?.ViewModel == default) return;
        
        _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.EditUserView;
    }
}