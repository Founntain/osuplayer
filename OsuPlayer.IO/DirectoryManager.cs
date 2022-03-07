namespace OsuPlayer.IO;

public static class DirectoryManager
{
    public static void GenerateMissingDirectories()
    {
        if (!Directory.Exists("data"))
            Directory.CreateDirectory("data");
    }
}