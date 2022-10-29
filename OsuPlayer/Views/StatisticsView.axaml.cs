using System.Reactive.Disposables;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class StatisticsView : ReactiveControl<StatisticsViewModel>
{
    public StatisticsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        this.WhenActivated(Block);
    }

    private void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);
    }
}