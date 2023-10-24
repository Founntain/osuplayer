namespace OsuPlayer.Data.LazerModels.Interfaces;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
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