using Avalonia;
using Avalonia.Controls;
using ReactiveUI;

namespace OsuPlayer.Views;

/// <summary>
/// A reactive player control class for all controls in our app
/// </summary>
/// <typeparam name="TViewModel">the ViewModel which the <see cref="ReactiveControl{TViewModel}" /> is bound to</typeparam>
public class ReactiveControl<TViewModel> : UserControl, IViewFor<TViewModel> where TViewModel : class
{
    public static readonly StyledProperty<TViewModel> ViewModelProperty = AvaloniaProperty
        .Register<ReactiveControl<TViewModel>, TViewModel>(nameof(ViewModel));

    object? IViewFor.ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public TViewModel ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public ReactiveControl()
    {
        this.WhenActivated(disposables => { });
        var x = this.GetObservable(ViewModelProperty);
        x.Subscribe(OnViewModelChanged);
    }

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