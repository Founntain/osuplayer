using Avalonia.Interactivity;
using Nein.Base;
using Nein.Extensions;

namespace OsuPlayer.Views;

public partial class UpdateView : ReactiveControl<UpdateViewModel>
{
    public UpdateView()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.Update?.HtmlUrl == default) return;

        GeneralExtensions.OpenUrl(ViewModel.Update.HtmlUrl);
    }
}