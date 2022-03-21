using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.Network.Online;

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

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var viewer = this.FindControl<ContentPresenter>("BadgeItems");
        var list = (ListBox) sender;
        
        if (list?.SelectedItem == default) return;
        
        var items = ViewModel!.LoadBadges((User) list.SelectedItem);

        viewer.Content = new ItemsRepeater
        {
            Items = items,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
            Layout = new WrapLayout {Orientation = Orientation.Horizontal}
        };
        
        viewer.UpdateChild();
    }
}