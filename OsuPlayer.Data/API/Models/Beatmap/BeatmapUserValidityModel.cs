using OsuPlayer.Data.API.Models.User;

namespace OsuPlayer.Data.API.Models.Beatmap
{
    public sealed class BeatmapUserValidityModel
    {
        public int Id { get; set; }
        public DateTime LastModified { get; set; }
        public UserModel User { get; set; }
        public BeatmapModel Beatmap { get; set; }
        public int TimesPlayed { get; set; }

        public string GetTitle => ToString();

        public string GetTimesplayed => $"{TimesPlayed} times played";

        public override string ToString()
        {
            return $"{Beatmap.Artist} - {Beatmap.Title}";
        }
    }
}