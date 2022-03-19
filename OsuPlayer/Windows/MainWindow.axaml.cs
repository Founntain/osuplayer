using System.ComponentModel;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.IO.Storage.Config;
using ReactiveUI;

namespace OsuPlayer.Windows;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            Core.Instance.SetupCore(this);
            
            if (Core.Instance.MainWindow.ViewModel != null)
                Core.Instance.MainWindow.ViewModel.MainView = Core.Instance.MainWindow.ViewModel.HomeView;
        });
        
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        using var config = new Config();
        var configContainer = config.Read();
        configContainer.Volume = Core.Instance.Player.Volume;
    }
}