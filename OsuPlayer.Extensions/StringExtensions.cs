namespace OsuPlayer.Extensions;

public static class StringExtensions
{
    public static string? IsNullOrWhiteSpaceWithFallback(this string? str, string? fallback) => string.IsNullOrWhiteSpace(str) ? fallback : str;
}