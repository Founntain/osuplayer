using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace OsuPlayer.UI_Extensions;

public partial class MessageBoxWindow : ReactiveWindow<MessageBoxViewModel>
{
    public MessageBoxWindow()
    {
        InitializeComponent();

        ViewModel = new MessageBoxViewModel();

        using var config = new Config();
        TransparencyLevelHint = config.Read().TransparencyLevelHint;

#if DEBUG
        this.AttachDevTools();
#endif
    }

    public MessageBoxWindow(string text, string? title)
    {
        InitializeComponent();

        ViewModel = new MessageBoxViewModel(this, text, title);

        using var config = new Config();
        TransparencyLevelHint = config.Read().TransparencyLevelHint;
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables => { });

        AvaloniaXamlLoader.Load(this);
    }
}