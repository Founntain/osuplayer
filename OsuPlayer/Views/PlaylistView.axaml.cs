using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

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