namespace OsuPlayer.Data.API.Models.User
{
    public sealed class UpdateXpModel
    {
        public string Username { get; set; }
        public string SongChecksum { get; set; }
        public double ElapsedMilliseconds { get; set; }
        public double ChannelLength { get; set; }
    }
}