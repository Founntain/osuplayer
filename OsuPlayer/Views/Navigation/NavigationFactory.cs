using Avalonia.Controls;
using FluentAvalonia.UI.Controls;

namespace OsuPlayer.Views.Navigation;

public class NavigationFactory : INavigationPageFactory
{
    public Control GetPage(Type srcType)
    {
        return null;
    }

    public Control GetPageFromObject(object target)
    {
        switch (target)
        {
            case HomeViewModel:
                return new HomeView();
            case PlaylistViewModel:
                return new PlaylistView();
            default:
                throw new ArgumentException();
        }
    }
}