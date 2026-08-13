using System.Text.RegularExpressions;

namespace OsuPlayer.Services;

public sealed record LrcLine(TimeSpan Time, string Text, string Translation);

public static class LrcParser
{
    private static readonly Regex TimestampRegex = new(
        @"\[(?<minutes>\d{1,3}):(?<seconds>\d{1,2})(?:[\.:](?<fraction>\d{1,3}))?\]",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex OffsetRegex = new(
        @"\[offset:(?<offset>[+-]?\d+)\]",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static IReadOnlyList<LrcLine> Parse(string lyrics, string translatedLyrics = "")
    {
        var primary = ParseTrack(lyrics);
        if (primary.Count == 0) return Array.Empty<LrcLine>();

        var translations = ParseTrack(translatedLyrics)
            .GroupBy(x => x.Time)
            .ToDictionary(x => x.Key, x => x.Last().Text);

        return primary
            .GroupBy(x => x.Time)
            .Select(group => group.LastOrDefault(x => !string.IsNullOrWhiteSpace(x.Text)) ?? group.Last())
            .Where(x => !string.IsNullOrWhiteSpace(x.Text))
            .OrderBy(x => x.Time)
            .Select(x => new LrcLine(x.Time, x.Text,
                translations.TryGetValue(x.Time, out var translation) ? translation : string.Empty))
            .ToArray();
    }

    public static int FindLineIndex(IReadOnlyList<LrcLine> lines, TimeSpan position)
    {
        if (lines.Count == 0 || position < lines[0].Time) return -1;

        var low = 0;
        var high = lines.Count - 1;
        while (low <= high)
        {
            var middle = low + ((high - low) / 2);
            if (lines[middle].Time <= position)
                low = middle + 1;
            else
                high = middle - 1;
        }

        return high;
    }

    private static List<RawLrcLine> ParseTrack(string source)
    {
        if (string.IsNullOrWhiteSpace(source)) return new List<RawLrcLine>();

        var offsetMatch = OffsetRegex.Match(source);
        var offset = offsetMatch.Success && int.TryParse(offsetMatch.Groups["offset"].Value, out var milliseconds)
            ? TimeSpan.FromMilliseconds(milliseconds)
            : TimeSpan.Zero;
        var result = new List<RawLrcLine>();

        foreach (var sourceLine in source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
        {
            var matches = TimestampRegex.Matches(sourceLine);
            if (matches.Count == 0) continue;

            var text = TimestampRegex.Replace(sourceLine, string.Empty).Trim();
            foreach (Match match in matches)
            {
                if (!int.TryParse(match.Groups["minutes"].Value, out var minutes) ||
                    !int.TryParse(match.Groups["seconds"].Value, out var seconds))
                    continue;

                var fractionText = match.Groups["fraction"].Value;
                var fractionMilliseconds = fractionText.Length switch
                {
                    1 => int.Parse(fractionText) * 100,
                    2 => int.Parse(fractionText) * 10,
                    3 => int.Parse(fractionText),
                    _ => 0
                };
                var time = TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds) +
                           TimeSpan.FromMilliseconds(fractionMilliseconds) + offset;
                result.Add(new RawLrcLine(time < TimeSpan.Zero ? TimeSpan.Zero : time, text));
            }
        }

        return result.OrderBy(x => x.Time).ToList();
    }

    private sealed record RawLrcLine(TimeSpan Time, string Text);
}
