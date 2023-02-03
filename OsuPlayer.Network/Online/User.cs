using System.Globalization;
using Avalonia.Media;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Api.Data.API.Enums;

namespace OsuPlayer.Network.Online;

/// <summary>
/// Represents a osu!player user
/// </summary>
public sealed class User : UserModel
{
    public string SongsPlayedString
    {
        get
        {
            if (UniqueId == Guid.Empty) return string.Empty;
                
            return SongsPlayed == default ? "0 songs played" : $"{SongsPlayed.ToString("##,###", CultureInfo.InvariantCulture)} songs played";
        }
    }

    public string LevelAndTotalXpString
    {
        get
        {
            if (UniqueId == Guid.Empty) return string.Empty;

            return TotalXp == default ? "Level 1 | Total XP: 0" : $"Level {Level.ToString("##,###", CultureInfo.InvariantCulture)} | Total XP: {TotalXp.ToString("##,###", CultureInfo.InvariantCulture)}";
        }
    }

    public string LevelProgressString
    {
        get
        {
            if (UniqueId == Guid.Empty) return string.Empty;
            
            return Xp == default 
                ? $"0 XP / {GetXpNeededForNextLevel().ToString("##,###", CultureInfo.InvariantCulture)} XP" 
                : $"{Xp.ToString("##,###", CultureInfo.InvariantCulture)} XP / {GetXpNeededForNextLevel().ToString("##,###", CultureInfo.InvariantCulture)} XP";
        }
    }

    public Brush RoleColor => GetRoleColorBrush();
    public string RoleString => GetRoleString();

    public string DescriptionTitleString => string.IsNullOrWhiteSpace(Description) ? $"{Name} has no description" : $"{Name}'s Description";
    public string LevelString => $"Level {Level}";
    public string JoinDateString => $"joined {JoinDate.ToString("D", new CultureInfo("en-us"))}";

    public string TotalXpString =>
        TotalXp == 0 ? "0 XP" : $"{TotalXp.ToString("##,###", CultureInfo.InvariantCulture)} XP";

    public User()
    {
        Role = UserRole.User;
    }

    public User(UserModel userModel)
    {
        UniqueId = userModel.UniqueId;
        Name = userModel.Name;
        Description = userModel.Description;
        Role = userModel.Role;
        JoinDate = userModel.JoinDate;
        LastActivity = userModel.LastActivity;
        LastSeen = userModel.LastSeen;
        Level = userModel.Level;
        Xp = userModel.Xp;
        TotalXp = userModel.TotalXp;
        HasXpLock = userModel.HasXpLock;
        SongsPlayed = userModel.SongsPlayed;
        OsuProfile = userModel.OsuProfile;

        IsDonator = userModel.IsDonator;
        AmountDonated = userModel.AmountDonated;
        CustomRolename = userModel.CustomRolename;
        CustomRoleColor = userModel.CustomRoleColor;
        CustomBannerUrl = userModel.CustomBannerUrl;

        Version = userModel.Version;

        Badges = userModel.Badges;
    }

    public override string? ToString()
    {
        return Name;
    }

    public override int GetHashCode()
    {
        return UniqueId.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is User user) return user.Name.Equals(Name);

        return false;
    }

    public int GetXpNeededForNextLevel()
    {
        return (int) Math.Round(0.04 * Math.Pow(Level, 3) + 0.8 * Math.Pow(Level, 2) + 2 * Level);
    }

    public static int GetXpNeededForNextLevel(int level)
    {
        return (int) Math.Round(0.04 * Math.Pow(level, 3) + 0.8 * Math.Pow(level, 2) + 2 * level);
    }

    public Brush GetRoleColorBrush()
    {
        switch (Role)
        {
            case UserRole.Staff:
                return UserColors.Staff;
            case UserRole.Developer:
                return UserColors.Developer;
            case UserRole.Tester:
                return UserColors.Tester;
            case UserRole.Donator:
                return UserColors.Donator;
            default:
                return UserColors.User;
        }
    }

    public string GetRoleString()
    {
        switch (Role)
        {
            case UserRole.Developer:
                return "Developer";
            case UserRole.Staff:
                return "Staff";
            case UserRole.Tester:
                return "Tester";
            case UserRole.Donator:
                return "Donator";
            default:
                return "User";
        }
    }
}