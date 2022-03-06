using System.Collections.Generic;
using OsuPlayerPlus.Classes.Online;

namespace OsuPlayerPlus.Classes.API;

public class ApiLanguage
{
    public string LanguageName { get; set; }
    public User Creator { get; set; }
    public int KeyCount { get; set; }
    public string CultureCode { get; set; }
    public Dictionary<string, string> LanguageData { get; set; }
    public bool ForceUpload { get; set; }
}