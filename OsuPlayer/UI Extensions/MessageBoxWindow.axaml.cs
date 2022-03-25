using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OsuPlayer.IO.Storage.Config;

namespace OsuPlayer.UI_Extensions;

public partial class MessageBoxWindow : Window
{
    public MessageBoxWindow()
    {
        InitializeComponent();
        
        DataContext = new MessageBoxViewModel();

        using var config = new Config();
        TransparencyLevelHint = config.Read().TransparencyLevelHint;

#if DEBUG
        this.AttachDevTools();
#endif
    }

    public MessageBoxWindow(string text, string? title)
    {
        InitializeComponent();

        DataContext = new MessageBoxViewModel(this, text, title);
#if DEBUG
        this.AttachDevTools();
#endif
    }


    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}