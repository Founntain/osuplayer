using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.Data.OsuPlayer.Enums;
using ReactiveUI;

namespace OsuPlayer.UI_Extensions;

public partial class MessageBoxWindow : ReactiveWindow<MessageBoxViewModel>
{
    public MessageBoxWindow()
    {
        InitializeComponent();

        ViewModel = new MessageBoxViewModel();

        var config = new Config();
        TransparencyLevelHint = config.Container.BackgroundMode.ToWindowTransparencyLevelList();

#if DEBUG
        this.AttachDevTools();
#endif
    }

    public MessageBoxWindow(string text, string? title)
    {
        InitializeComponent();

        ViewModel = new MessageBoxViewModel(this, text, title);

        var config = new Config();
        TransparencyLevelHint = config.Container.BackgroundMode.ToWindowTransparencyLevelList();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        this.WhenActivated(_ => { });

        AvaloniaXamlLoader.Load(this);
    }
}