using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.Audio;
using OsuPlayer.Data;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

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
            new Core(this);
            Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.HomeView;
        });
        AvaloniaXamlLoader.Load(this);
    }

    private void SongProgressSlider_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        //throw new NotImplementedException();
    }

    private void SongProgressSlider_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        //throw new NotImplementedException();
    }
}