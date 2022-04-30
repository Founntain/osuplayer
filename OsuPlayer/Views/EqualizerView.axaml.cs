using Avalonia.Controls;
using Avalonia.Interactivity;
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

    private void ResetSlider(object? sender, RoutedEventArgs e)
    {
        if (sender is Slider slider)
            slider.Value = 0;
    }
}