using System;
using System.Collections.Generic;

namespace OsuPlayer.Data.API.Models.Statistic
{
    public class ApiStatisticsV3Model
    {
        public uint TotalUserCount { get; set; }
        public IList<Tuple<string, int, int>> Activity { get; set; }
        public IList<int> OldActivity { get; set; }
        public IList<Tuple<string, int>> RegisteredUsers { get; set; }
        public uint TranslatorCount { get; set; }
        public ulong TotalSongsPlayed { get; set; }
        public ulong CommunityTotalXp { get; set; }
        public uint CommunityLevel { get; set; }
        public ulong CommunityXpLeft { get; set; }
        public Tuple<string, ulong> UserWithMostXp { get; set; }
    }
}