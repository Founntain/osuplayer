using Avalonia.Platform.Storage;

namespace OsuPlayer.Extensions;

public class FilePickerFileTypesExtensions {
    public static FilePickerFileType OsuDb { get; } = new("osu! db files")
    {
        Patterns = new[] { "osu!.db", "client.realm", },
        MimeTypes = null
    };
}