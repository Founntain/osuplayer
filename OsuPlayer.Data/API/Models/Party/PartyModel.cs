using OsuPlayer.Data.API.Models.User;

namespace OsuPlayer.Data.API.Models.Party
{
    public sealed class PartyModel
    {
        public Guid PartyId { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsPublic { get; set; }
        public string Song { get; set; }
        public string Songname { get; set; }
        public string Hostname { get; set; }
        public bool IsPaused { get; set; }
        public double Timestamp { get; set; }
        public double Speed { get; set; }
        public ICollection<ClientModel> Clients { get; set; }

        public override string ToString()
        {
            return $"{Hostname}'s party with {Clients.Count} party members";
        }
    }
}