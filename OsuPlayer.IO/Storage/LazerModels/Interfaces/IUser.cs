namespace OsuPlayer.IO.Storage.LazerModels.Interfaces;

public interface IUser : IHasOnlineID<int>, IEquatable<IUser>
{
    string Username { get; }

    bool IsBot { get; }

    bool IEquatable<IUser>.Equals(IUser? other)
    {
        if (other == null)
            return false;

        return OnlineID == other.OnlineID && Username == other.Username;
    }
}