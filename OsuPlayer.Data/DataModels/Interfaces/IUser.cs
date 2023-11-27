using Avalonia.Media;
using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Data.DataModels.Interfaces;

public interface IUser
{
    public Guid UniqueId { get; }

    public int Xp { get; set; }
    public int XpNeededForNextLevel { get; }
    public int Level { get; set; }

    public string XpFormattedString { get; }
    public string TotalXpFormattedString { get; }
    public string SongsPlayedFormattedString { get; }

    public string SongsPlayedComplexString { get; }
    public string LevelAndTotalXpString { get; }
    public string LevelProgressString { get; }
    public Brush RoleColor { get; }
    public string RoleString { get; }
    public string DescriptionTitleString { get; }
    public string LevelString { get; }
    public string JoinDateString { get; }
    public string TotalXpString { get; }

    public List<BadgeModel> Badges { get; set; }

    public int GetXpNeededForNextLevel();
    public static abstract int GetXpNeededForNextLevel(int level);
    public Brush GetRoleColorBrush();
    public string GetRoleString();
}