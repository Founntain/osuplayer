using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Nein.Base;

namespace OsuPlayer.Views;

public partial class PartyView : ReactiveControl<PartyViewModel>
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