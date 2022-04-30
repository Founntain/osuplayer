using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OsuPlayer.Views;

public partial class EqualizerView : ReactivePlayerControl<EqualizerViewModel>
{
    public EqualizerView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}