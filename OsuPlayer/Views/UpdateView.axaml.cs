using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OsuPlayer.Extensions;

namespace OsuPlayer.Views;

public partial class UpdateView : ReactivePlayerControl<UpdateViewModel>
{
    public UpdateView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel?.UpdateUrl == default) return;

        GeneralExtensions.OpenUrl(ViewModel.UpdateUrl);
    }
}