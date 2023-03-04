using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Material.Icons;
using OsuPlayer.Controls.Interfaces;

namespace OsuPlayer.Controls;

public partial class SettingsTitleDisplayer : UserControl, ISettingsDisplayer
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<SettingsDisplayer, string>(nameof(Title));

    public string Title
    {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly StyledProperty<MaterialIconKind> IconProperty =
        AvaloniaProperty.Register<SettingsDisplayer, MaterialIconKind>(nameof(Icon));

    public MaterialIconKind Icon
    {
        get { return GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public SettingsTitleDisplayer()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);

        RefreshCorners();
    }

    public void RefreshCorners()
    {
        var parent = this.GetLogicalParent();

        var parentChildren = parent.GetLogicalChildren().Where(x => (x as Visual)?.IsVisible ?? true).ToList();

        PseudoClasses.Remove(":first-child");
        PseudoClasses.Remove(":only-child");
        PseudoClasses.Remove(":last-child");

        if (parentChildren.Count == 1)
        {
            PseudoClasses.Add(":only-child");
            return;
        }

        var position = parentChildren.IndexOf(this);

        if (position == 0)
            PseudoClasses.Add(":first-child");

        if (position == parentChildren.Count - 1)
            PseudoClasses.Add(":last-child");
    }
}