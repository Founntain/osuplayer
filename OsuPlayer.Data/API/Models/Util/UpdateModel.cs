using System;

namespace OsuPlayer.Data.API.Models.Util
{
    public sealed class UpdateModel
    {
        public string ZipFile { get; set; }
        public int NewVersion { get; set; }
        public DateTime NewVersionDate { get; set; }
    }
}