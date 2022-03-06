using System;
using OsuPlayer.Data.API.Enums;

namespace OsuPlayer.Data.API.Models.User
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public UserRole Role { get; set; }
        public DateTime JoinDate { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }
        public int TotalXp { get; set; }
        public bool HasXpLock { get; set; }
        public int Version { get; set; }
        public DateTime VersionDate { get; set; }
        public DateTime LastSeen { get; set; }
        public string OsuProfile { get; set; }
        public int SongsPlayed { get; set; }
        public bool IsDonator { get; set; }
        public double AmountDonated { get; set; }
        public string CustomRolename { get; set; }
        public string CustomRoleColor { get; set; }
        public string CustomWebBackground { get; set; }

        public string ProfilePicture { get; set; }
    }
}