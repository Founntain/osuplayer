using System.Collections.Generic;
using OsuPlayer.Modules.Network.Online;

namespace OsuPlayer.Modules.Network.API;

public class ApiLanguage
{
    public string LanguageName { get; set; }
    public User Creator { get; set; }
    public int KeyCount { get; set; }
    public string CultureCode { get; set; }
    public Dictionary<string, string> LanguageData { get; set; }
    public bool ForceUpload { get; set; }
}