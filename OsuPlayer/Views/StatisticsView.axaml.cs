using System.Reactive.Disposables;
using Nein.Base;

namespace OsuPlayer.Views;

public partial class StatisticsView : ReactiveControl<StatisticsViewModel>
{
    public StatisticsView()
    {
        InitializeComponent();
    }

    private void Block(Action<IDisposable> disposables)
    {
        disposables(Disposable.Create(() => { }));
    }
}