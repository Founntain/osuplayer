using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.ViewModels;

namespace OsuPlayer.Views;

public partial class PlaylistView : ReactiveUserControl<PlaylistViewModel>
{
    public PlaylistView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);  
    }
}