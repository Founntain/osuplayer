using MongoDB.Bson;
using Realms;

namespace OsuPlayer.Data.OsuPlayer.Database.Entities;

public class Song : RealmObject
{
    [PrimaryKey]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    public DateTimeOffset CreationTime { get; set; } = DateTime.UtcNow;
    
    public string Songchecksum { get; set; }
    
    public IList<Playlist> Playlists { get; }
}