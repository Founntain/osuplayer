using System;
using Avalonia;
using Avalonia.Controls;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class ReactivePlayerControl<TViewModel> : UserControl, IViewFor<TViewModel> where TViewModel : BaseViewModel
{
    public static readonly StyledProperty<TViewModel> ViewModelProperty = AvaloniaProperty
        .Register<ReactivePlayerControl<TViewModel>, TViewModel>(nameof(ViewModel));

    public ReactivePlayerControl()
    {
        this.WhenActivated(disposables => { ViewModel!.Activator.Activate(); });
        var x = this.GetObservable(ViewModelProperty);
        x.Subscribe(OnViewModelChanged);
    }


    object? IViewFor.ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public TViewModel ViewModel { get; set; }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        ViewModel = (DataContext as TViewModel)!;
    }

    private void OnViewModelChanged(object? value)
    {
        if (value == null)
            ClearValue(DataContextProperty);
        else if (DataContext != value) DataContext = value;
    }
}