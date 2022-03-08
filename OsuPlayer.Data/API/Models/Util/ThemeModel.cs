namespace OsuPlayer.Data.API.Models.Util;

public sealed class ThemeModel
{
    public string Username { get; set; }
    public string Themename { get; set; }
    public string BaseColor { get; set; }
    public string SecondaryColor { get; set; }
    public string ControlsColor { get; set; }
    public string AccentColor { get; set; }
    public bool UseDarkFont { get; set; }
    public bool UseDarkIcons { get; set; }
    public bool UseDarkControlIcons { get; set; }
    public bool UseCustomBackground { get; set; }
}