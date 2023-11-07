using System.Reactive.Disposables;
using Avalonia.Markup.Xaml;
using Nein.Base;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class StatisticsView : ReactiveControl<StatisticsViewModel>
{
    public StatisticsView()
    {
        InitializeComponent();
    }

    private void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);
    }
}