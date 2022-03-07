namespace OsuPlayer.IO;

public static class OsuPlayerConfig
{
    public static string OsuPath { get; set; }
    public static string OsuSongsPath => $"{OsuPath}\\Songs";
    
    public static bool UseSongnameUnicode { get; set; }
    
    public static int SelectedOutputDevice { get; set; }
    public static bool IsEqEnabled { get; set; }
}