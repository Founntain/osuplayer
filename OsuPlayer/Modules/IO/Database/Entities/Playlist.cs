using System.Collections.Generic;

namespace OsuPlayer.Modules.IO.Database.Entities;

public class Playlist : BaseEntity
{
    public string Name { get; set; }

    public virtual HashSet<string> Songs { get; set; }
}