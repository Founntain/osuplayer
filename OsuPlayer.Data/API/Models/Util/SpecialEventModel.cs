using System;

namespace OsuPlayer.Data.API.Models.Util
{
    public sealed class SpecialEventModel
    {
        public int Id { get; set; }
        public bool IsTimespan { get; set; }
        public bool Repeating { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }
        public int XpModifier { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}