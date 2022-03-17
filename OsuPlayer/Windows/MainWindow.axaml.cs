using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.ViewModels;
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
        Core.Instance.Config.SaveConfig();
        base.OnClosing(e);
    }
}