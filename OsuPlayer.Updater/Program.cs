﻿namespace OsuPlayer.Updater;

public static class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No URL found. To close, press any key.");

            Console.ReadKey();
            return;
        }

        var url = args[0];

        Console.WriteLine($"Updating with binary from: {url}");
        Console.WriteLine($"Press key to continue or close the window to cancel");

        Console.ReadKey();

        Console.WriteLine("Updating now!");

        using (var client = new HttpClient())
        {
            var updateZipStream = await client.GetStreamAsync(url);

            if (!updateZipStream.CanRead)
                return;

            if (Directory.Exists("update_temp"))
                Directory.Delete("update_temp", true);

            System.IO.Compression.ZipFile.ExtractToDirectory(updateZipStream, "update_temp");
        }

        var filesRaw = Directory.EnumerateFiles("update_temp").ToList();

        bool containsUpdaterFiles = filesRaw.Any(x => x.Contains("OsuPlayer.Updater"));

        var files = filesRaw.Where(x => !x.Contains("OsuPlayer.Updater"));

        var oldFiles = Directory.EnumerateFiles(".").ToList().Where(x => !x.Contains("OsuPlayer.Updater"));

        foreach (string oldFile in oldFiles)
        {
            try
            {
                File.Delete(oldFile);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting old file {oldFile} with exception: {e.Message}");
                Console.WriteLine(
                    "Please delete all old files (not the folders!) and copy the content from 'update_temp' manually into your osu!player directory");

                Console.ReadKey();

                return;
            }
        }

        foreach (string file in files)
        {
            try
            {
                var newPath = Path.GetFileName(file);

                File.Delete(newPath);

                File.Move(file, newPath, true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{file} has error with exception: {e.Message}");
                Console.WriteLine("Please restart the updater and make sure osu!player has quit.");

                Console.ReadKey();
                return;
            }
        }

        if (!containsUpdaterFiles)
            Directory.Delete("update_temp");

        Console.WriteLine("Updating finished successfully. Please restart the osu!player manually. Press any key to quit.");

        Console.ReadLine();
    }
}