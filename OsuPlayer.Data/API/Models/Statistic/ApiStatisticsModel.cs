using OsuPlayer.Data.API.Enums;
using OsuPlayer.Data.API.Models.User;

namespace OsuPlayer.Data.API.Models.Statistic
{
    public sealed class ApiStatisticsModel
    {
        public int TotalUserCount { get; set; }
        public IList<Tuple<UserRole, int>> UsersByRole { get; set; }
        public UserModel LatestUser { get; set; }
        public IList<int> Activity { get; set; }
        public IList<Tuple<string, int>> RegisteredUsers { get; set; }
        public IList<Tuple<string, string>> Translators { get; set; }
        public int TotalSongsPlayed { get; set; }
        public int TotalXp { get; set; }
    }
}