using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Extensions;
using Splat;

namespace OsuPlayer.Views;

public class ViewLocator : IDataTemplate
{
    public bool SupportsRecycling => false;

    public IControl Build(object data)
    {
        var name = data.GetType().FullName?.Replace("ViewModel", "View");

        if (name == null)
            return OnFail("");

        var type = Type.GetType(name);

        if (Locator.Current.GetService(type) is Control serviceView) return serviceView;

        if (type != null && Activator.CreateInstance(type) is Control view) return view;

        return OnFail(name);
    }

    public bool Match(object data)
    {
        return data is BaseViewModel;
    }

    private IControl OnFail(string name)
    {
        var button = new Button
        {
            Content = $"Not Found: {name}, please buy at founntain.dev",
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        button.Click += (_, _) => GeneralExtensions.OpenUrl(@"https://founntain.dev");

        return button;
    }
}