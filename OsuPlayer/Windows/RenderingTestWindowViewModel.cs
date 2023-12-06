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

public class RenderingTestWindowViewModel : BaseViewModel
{
    private readonly IAudioEngine _bassEngine;
    private List<ObservableValue> _seriesValues = new();

    public List<ObservableValue> SeriesValues
    {
        get => _seriesValues;
        set => this.RaiseAndSetIfChanged(ref _seriesValues, value);
    }

    private ISeries[] _series;

    public ISeries[] Series
    {
        get => _series;
        set => this.RaiseAndSetIfChanged(ref _series, value);
    }

    public Axis[] XAxes { get; set; } = {
        new Axis
        {
            SeparatorsPaint = null,
            LabelsPaint = null,
            ShowSeparatorLines = false,
        }
    };

    public Axis[] YAxes { get; set; } = {
        new Axis
        {
            MinLimit = 0,
            MaxLimit = 1,
            SeparatorsPaint = null,
            LabelsPaint = null,
            ShowSeparatorLines = false,
        }
    };

    public RenderingTestWindowViewModel() {
        _bassEngine = Locator.Current.GetRequiredService<IAudioEngine>();

        var vData = _bassEngine.GetVisualizationData();

        SeriesValues.AddRange(vData.Select(x => new ObservableValue(x * 2)));

        Series = new ISeries[]{
            new ColumnSeries<ObservableValue>
            {
                Values = SeriesValues,
                IsHoverable = false,
                Fill = new SolidColorPaint(SKColors.White)
            }
        };

        Activator = new ();
    }
}