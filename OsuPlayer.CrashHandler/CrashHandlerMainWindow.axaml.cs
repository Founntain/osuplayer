using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Nein.Base;
using Nein.Extensions;

namespace OsuPlayer.CrashHandler;

public partial class CrashHandlerMainWindow : ReactiveWindow<CrashHandlerMainWindowViewModel>
{
    public CrashHandlerMainWindow()
    {
        ViewModel = new CrashHandlerMainWindowViewModel();

        InitializeComponent();

        // this.WhenActivated(_ => { });
    }

    private async void Window_OnActivated(object? sender, EventArgs e)
    {
        if (ViewModel == default) return;

        var files = Directory.GetFiles("logs").Select(x => new FileInfo(x));

        var latestLog = files.MaxBy(x => x.CreationTimeUtc);

        if (latestLog == default) return;

        var crashLog = await File.ReadAllTextAsync(latestLog.FullName);

        ViewModel.CrashLog = crashLog;
    }

    private async void Copy_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == null) return;

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel == default || topLevel.Clipboard == default) return;

        var transfer = new DataTransfer();
        var transferItem = new DataTransferItem();

        transferItem.SetText(ViewModel.CrashLog);
        transfer.Add(transferItem);

        await topLevel.Clipboard.SetDataAsync(transfer);
    }

    private void GitHub_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl(@"https://github.com/osu-player/osuplayer/issues/new?assignees=&labels=bug&template=bug_report.md&title=%5BBUG%5D+");
    }

    private void Discord_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl(@"https://discord.gg/RJQSc5B");
    }

    private void Email_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl(@"mailto:7@founntain.dev");
    }
}