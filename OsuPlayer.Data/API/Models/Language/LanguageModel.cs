using OsuPlayer.Data.API.Models.User;

namespace OsuPlayer.Data.API.Models.Language;

public sealed class LanguageModel
{
    public DateTime CreationTime { get; set; }
    public string LanguageName { get; set; }
    public UserModel Creator { get; set; }
    public DateTime LastModified { get; set; }
    public string ModifiedBy { get; set; }
    public int KeyCount { get; set; }
    public string CultureCode { get; set; }
    public Dictionary<string, string> LanguageData { get; set; }
    public bool ForceUpload { get; set; }
}