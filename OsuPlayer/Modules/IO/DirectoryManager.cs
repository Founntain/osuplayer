using System.IO;

namespace OsuPlayer.Modules.IO;

public static class DirectoryManager
{
    public static void GenerateMissingDirectories()
    {
        if (!Directory.Exists("data"))
            Directory.CreateDirectory("data");
    }
}