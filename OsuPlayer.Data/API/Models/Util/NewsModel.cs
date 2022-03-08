namespace OsuPlayer.Data.API.Models.Util
{
    public sealed class NewsModel
    {
        public int Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string Title { get; set; }
        public string Creator { get; set; }
        public string Content { get; set; }
    }
}