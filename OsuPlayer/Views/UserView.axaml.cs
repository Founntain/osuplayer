using System.Threading;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace OsuPlayer.Views;

public partial class UserView : ReactiveUserControl<UserViewModel>
{
    private CancellationTokenSource? CancellationTokenSource;
    
    public UserView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}