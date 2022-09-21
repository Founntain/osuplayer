using Avalonia;
using Avalonia.Controls;
using ReactiveUI;

namespace OsuPlayer.Base.ViewModels;

/// <summary>
/// A reactive window class for all windows in our app
/// </summary>
/// <typeparam name="TViewModel">the ViewModel which the <see cref="ReactiveWindow{TViewModel}" /> is bound to</typeparam>
public class ReactiveWindow<TViewModel> : Window, IViewFor<TViewModel> where TViewModel : ReactiveObject
{
    public static readonly StyledProperty<TViewModel> ViewModelProperty = AvaloniaProperty
        .Register<ReactiveWindow<TViewModel>, TViewModel>(nameof(ViewModel));

    public ReactiveWindow()
    {
        this.WhenActivated(disposables => { });
        this.GetObservable(DataContextProperty).Subscribe(OnDataContextChanged);
        this.GetObservable(ViewModelProperty).Subscribe(OnViewModelChanged);
    }

    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel) value;
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
        else if (DataContext != value)
            DataContext = value;
    }
}