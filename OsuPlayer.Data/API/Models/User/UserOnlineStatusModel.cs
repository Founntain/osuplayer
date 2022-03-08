using OsuPlayer.Data.API.Enums;

namespace OsuPlayer.Data.API.Models.User
{
    public class UserOnlineStatusModel
    {
        public string Username { get; set; }
        public DateTime LastUpdate { get; set; }
        public UserOnlineStatusType StatusType { get; set; }
        public string Song { get; set; }
        public string SongChecksum { get; set; }
    }
}