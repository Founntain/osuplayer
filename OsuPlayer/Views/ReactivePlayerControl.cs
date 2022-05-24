using Avalonia;
using Avalonia.Controls;
using ReactiveUI;

namespace OsuPlayer.Views;

/// <summary>
/// A reactive player control class for all controls in our app
/// </summary>
/// <typeparam name="TViewModel">the ViewModel which the <see cref="ReactivePlayerControl{TViewModel}"/> is bound to</typeparam>
public class ReactivePlayerControl<TViewModel> : UserControl, IViewFor<TViewModel> where TViewModel : class
{
    public static readonly StyledProperty<TViewModel> ViewModelProperty = AvaloniaProperty
        .Register<ReactivePlayerControl<TViewModel>, TViewModel>(nameof(ViewModel));

    public ReactivePlayerControl()
    {
        this.WhenActivated(disposables => { });
        var x = this.GetObservable(ViewModelProperty);
        x.Subscribe(OnViewModelChanged);
    }

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