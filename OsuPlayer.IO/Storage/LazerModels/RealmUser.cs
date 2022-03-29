using OsuPlayer.IO.Storage.LazerModels.Interfaces;
using Realms;

namespace OsuPlayer.IO.Storage.LazerModels;

public class RealmUser : EmbeddedObject, IUser, IEquatable<RealmUser>
{
    public bool Equals(RealmUser other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return OnlineID == other.OnlineID && Username == other.Username;
    }

    public int OnlineID { get; set; } = 1;

    public string Username { get; set; } = string.Empty;

    public bool IsBot => false;
}