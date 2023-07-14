using System;
using System.IO;
using System.Linq;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Nein.Base;
using Nein.Extensions;
using ReactiveUI;

namespace OsuPlayer.CrashHandler;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        ViewModel = new MainWindowViewModel();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(_ => { });

        AvaloniaXamlLoader.Load(this);
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

        var topLevel = GetTopLevel(this);
        
        if (topLevel?.Clipboard == default) return;

        await topLevel.Clipboard.SetTextAsync(ViewModel.CrashLog);
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