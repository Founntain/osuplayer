using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Material.Icons;
using OsuPlayer.Controls.Enums;

namespace OsuPlayer.Controls;

public partial class SettingsTitleDisplayer : UserControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<SettingsDisplayer, string>(nameof(Title));

    public string Title
    {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly StyledProperty<CornerRadius> CornerBehaviourProperty =
        AvaloniaProperty.Register<SettingsDisplayer, CornerRadius>(nameof(CornerBehaviour));
    
    public CornerRadius CornerBehaviour
    {
        get { return GetValue(CornerBehaviourProperty); }
        set { SetValue(CornerBehaviourProperty, value); }
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
}