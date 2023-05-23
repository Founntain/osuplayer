using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Animators;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
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