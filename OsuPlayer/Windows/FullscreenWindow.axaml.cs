using System.Runtime.InteropServices.JavaScript;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Modules.Audio.Interfaces;
using Splat;

namespace OsuPlayer.Windows;

public partial class FullscreenWindow : ReactiveWindow<FullscreenWindowViewModel>
{
    public FullscreenWindow()
    {
        InitializeComponent();

        DataContext = new FullscreenWindowViewModel(Locator.Current.GetRequiredService<IPlayer>());
        
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}