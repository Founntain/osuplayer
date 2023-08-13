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

    private void LoginBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}