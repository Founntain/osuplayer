using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Nein.Base;
using Nein.Extensions;

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

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl("https://github.com/osu-player/osuplayer/issues/70");
    }
}