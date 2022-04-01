using System;
using Avalonia;
using Avalonia.Controls;
using ReactiveUI;

namespace OsuPlayer.Windows;

public class ReactivePlayerWindow<TViewModel> : Window, IViewFor<TViewModel> where TViewModel : ReactiveObject
{
    public static readonly StyledProperty<TViewModel> ViewModelProperty = AvaloniaProperty
        .Register<ReactivePlayerWindow<TViewModel>, TViewModel>(nameof(ViewModel));


    public ReactivePlayerWindow()
    {
        this.WhenActivated(disposables => { });
        this.GetObservable(DataContextProperty).Subscribe(OnDataContextChanged);
        this.GetObservable(ViewModelProperty).Subscribe(OnViewModelChanged);
    }

    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel)value;
    }

    public TViewModel ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private void OnDataContextChanged(object? value)
    {
        if (value is TViewModel viewModel)
            ViewModel = viewModel;
        else
            ViewModel = null;
    }

    private void OnViewModelChanged(object? value)
    {
        if (value == null)
            ClearValue(DataContextProperty);
        else if (DataContext != value) DataContext = value;
    }
}