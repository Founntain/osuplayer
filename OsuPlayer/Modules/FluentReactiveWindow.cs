using Avalonia;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Windowing;
using ReactiveUI;

namespace OsuPlayer.Modules;

public class FluentReactiveWindow<TViewModel> : AppWindow, IViewFor<TViewModel>, IViewFor, IActivatableView where TViewModel : ReactiveObject
{
    public static readonly StyledProperty<TViewModel> ViewModelProperty = AvaloniaProperty.Register<FluentReactiveWindow<TViewModel>, TViewModel>(nameof (ViewModel));

    object? IViewFor.ViewModel
    {
        get => (object) this.ViewModel;
        set => this.ViewModel = (TViewModel) value;
    }

    public TViewModel? ViewModel
    {
        get => this.GetValue<TViewModel>(ReactiveWindow<TViewModel>.ViewModelProperty);
        set => this.SetValue<TViewModel>(ReactiveWindow<TViewModel>.ViewModelProperty, value);
    }

    public FluentReactiveWindow()
    {
        this.WhenActivated((Action<Action<IDisposable>>) (disposables => { }));
        this.GetObservable<object>((AvaloniaProperty<object>) StyledElement.DataContextProperty).Subscribe<object>(new Action<object>(this.OnDataContextChanged));
        this.GetObservable<TViewModel>((AvaloniaProperty<TViewModel>) ReactiveWindow<TViewModel>.ViewModelProperty).Subscribe<TViewModel>(new Action<TViewModel>(this.OnViewModelChanged));
    }

    private void OnDataContextChanged(object? value)
    {
        if (value is TViewModel viewModel)
            this.ViewModel = viewModel;
        else
            this.ViewModel = default (TViewModel);
    }

    private void OnViewModelChanged(object? value)
    {
        if (value == null)
        {
            this.ClearValue<object>(StyledElement.DataContextProperty);
        }
        else
        {
            if (this.DataContext == value)
                return;
            this.DataContext = value;
        }
    }
}