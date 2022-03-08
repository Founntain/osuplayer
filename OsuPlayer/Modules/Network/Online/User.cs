using System;
using Avalonia.Media;
using OsuPlayer.Data.API.Enums;
using OsuPlayer.Data.API.Models.User;

namespace OsuPlayer.Modules.Network.Online;

public sealed class User : UserModel
{
    public User()
    {
        Role = UserRole.Unknown;
    }

    public Brush RoleColor => GetRoleColorBrush();
    public string RoleString => GetRoleString();

    public override string ToString()
    {
        return Name;
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
            case UserRole.Developer:
                return UserColors.Developer;
            case UserRole.Tester:
                return UserColors.Tester;
            case UserRole.Translator:
                return UserColors.Translator;
            case UserRole.LegacyUser:
                return UserColors.LegacyUser;
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
            case UserRole.Tester:
                return "Tester";
            case UserRole.Translator:
                return "Translator";
            case UserRole.Donator:
                return "Donator";
            default:
                return "User";
        }
    }
}