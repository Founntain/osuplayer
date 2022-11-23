using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OsuPlayer.Views;

public partial class SymmetricalView : UserControl
{
    public SymmetricalView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}