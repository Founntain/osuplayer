using Avalonia.Controls;
using Avalonia.Interactivity;
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
    }
}