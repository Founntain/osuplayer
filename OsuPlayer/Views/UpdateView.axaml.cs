using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Interactivity;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using Splat;

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

    private async void Update_OnClick(object? sender, RoutedEventArgs e)
    {
        var asset = ViewModel.Update?.Assets?.FirstOrDefault(x => x.Name.Contains(RuntimeInformation.RuntimeIdentifier));

        if (asset == default)
        {
            await MessageBox.ShowDialogAsync(Locator.Current.GetRequiredService<FluentAppWindow>(),
                "No update for you os found, please visit github for manual update.", "Update binary not found!");

            return;
        }

        var processStartInfo = new ProcessStartInfo("dotnet", [
            "OsuPlayer.Updater.dll", asset.BrowserDownloadUrl
        ]);

        Process.Start(processStartInfo);
        
        Locator.Current.GetRequiredService<FluentAppWindow>().Close();
    }
}