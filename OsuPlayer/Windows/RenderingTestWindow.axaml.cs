using System.Diagnostics;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaEdit.Utils;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Modules.Audio.Interfaces;
using ReactiveUI;
using SkiaSharp;
using Splat;

namespace OsuPlayer.Windows;

public partial class RenderingTestWindow : FluentReactiveWindow<RenderingTestWindowViewModel>
{
    private DispatcherTimer _timer = new();

    public RenderingTestWindow()
    {
        InitializeComponent();

        ViewModel = new RenderingTestWindowViewModel();

        _timer.Interval = TimeSpan.FromMilliseconds(1);
        _timer.Tick += TimerOnTick;
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var audioEngine = Locator.Current.GetRequiredService<IAudioEngine>();

            var vData = audioEngine.GetVisualizationData();

            Debug.WriteLine(vData.First());

            ViewModel?.SeriesValues.Clear();

            ViewModel?.SeriesValues.AddRange(vData.Select(x => new ObservableValue(x * 2)));

            ((ColumnSeries<ObservableValue>) ViewModel!.Series.First() ).Values = ViewModel.SeriesValues;
        });
    }

    private void WindowBase_OnActivated(object? sender, EventArgs e)
    {
        _timer.Start();
    }
}