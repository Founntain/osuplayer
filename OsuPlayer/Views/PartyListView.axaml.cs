using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LiveChartsCore.Drawing;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Modules.Party;
using OsuPlayer.Windows;
using Splat;

namespace OsuPlayer.Views;

public partial class PartyListView : ReactiveControl<PartyListViewModel>
{
    public PartyListView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void JoinParty_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void CreateParty_OnClick(object? sender, RoutedEventArgs e)
    {
        var pm = Locator.Current.GetService<PartyManager>();

        var party = await pm.CreateParty();

        if (party == default) return;

        var mainWindow = Locator.Current.GetRequiredService<MainWindow>();

        mainWindow.ViewModel!.MainView = mainWindow.ViewModel.PartyView;
    }
}