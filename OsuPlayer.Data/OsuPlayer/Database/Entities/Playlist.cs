using MongoDB.Bson;
using Realms;

namespace OsuPlayer.Data.OsuPlayer.Database.Entities;

public class Playlist : RealmObject
{
    [PrimaryKey]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    
    public DateTimeOffset CreationTime { get; set; } = DateTime.UtcNow;
    
    public string Name { get; set; }

    public IList<Song> Songs { get; }
}