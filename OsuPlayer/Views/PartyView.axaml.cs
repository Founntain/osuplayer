using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace OsuPlayer.Views;

public partial class PartyView : UserControl
{
    public PartyView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}