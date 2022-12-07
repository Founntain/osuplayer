namespace OsuPlayer.Extensions;

public class NullOrEmptyException : Exception
{
    public NullOrEmptyException(string? message) : base(message)
    {
    }
}