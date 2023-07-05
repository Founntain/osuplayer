using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Nein.Base;

namespace OsuPlayer.Views;

public partial class BeatmapsView : ReactiveControl<BeatmapsViewModel>
{
    public BeatmapsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}