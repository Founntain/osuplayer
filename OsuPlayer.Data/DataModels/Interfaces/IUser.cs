using Avalonia.Media;

namespace OsuPlayer.Data.DataModels.Interfaces;

public interface IUser
{
    public Guid UniqueId { get; }

    public string SongsPlayedString { get; }
    public string LevelAndTotalXpString { get; }
    public string LevelProgressString { get; }
    public Brush RoleColor { get; }
    public string RoleString { get; }
    public string DescriptionTitleString { get; }
    public string LevelString { get; }
    public string JoinDateString { get; }
    public string TotalXpString { get; }

    public int GetXpNeededForNextLevel();
    public static abstract int GetXpNeededForNextLevel(int level);
    public Brush GetRoleColorBrush();
    public string GetRoleString();
}