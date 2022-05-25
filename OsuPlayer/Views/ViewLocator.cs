using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using OsuPlayer.ViewModels;

namespace OsuPlayer.Views;

public class ViewLocator : IDataTemplate
{
    public bool SupportsRecycling => false;

    public IControl Build(object data)
    {
        var name = data.GetType().FullName.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null) return (Control) Activator.CreateInstance(type);

        return new Button
        {
            Content = $"Not Found: {name}, please buy at founntain.dev",
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
    }

    public bool Match(object data)
    {
        return data is BaseViewModel;
    }
}