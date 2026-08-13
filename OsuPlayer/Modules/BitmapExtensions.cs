using Avalonia.Media.Imaging;
using AvaloniaColor = Avalonia.Media.Color;
using SkiaSharp;

namespace OsuPlayer.Modules;

public static class BitmapExtensions
{
    private readonly record struct ColorBucket(int Count, long Red, long Green, long Blue);

    public static Bitmap BlurBitmap(string imagePath, float blurRadius = 10f, float opacity = 1f, int quality = 80)
    {
        using var stream = File.OpenRead(imagePath);
        using var skBitmap = SKBitmap.Decode(stream);

        var blurSigma = blurRadius / 2;
        var blurFilter = SKImageFilter.CreateBlur(blurRadius, blurSigma);

        var paint = new SKPaint
        {
            ImageFilter = blurFilter,
            Color = new SKColor(alpha: (byte)(255 * opacity), red: 0, green: 0, blue: 0)
        };

        using var surface = SKSurface.Create(new SKImageInfo(skBitmap.Width, skBitmap.Height));
        var canvas = surface.Canvas;

        // Draw the original image with the blur effect
        canvas.DrawBitmap(skBitmap, 0, 0, paint);

        using var image = surface.Snapshot();
        using var outputStream = new MemoryStream();

        image.Encode(SKEncodedImageFormat.Png, quality).SaveTo(outputStream);
        outputStream.Position = 0;

        return new Bitmap(outputStream);
    }

    public static AvaloniaColor GetDominantColor(string imagePath)
    {
        using var stream = File.OpenRead(imagePath);
        using var bitmap = SKBitmap.Decode(stream);

        if (bitmap == null || bitmap.Width == 0 || bitmap.Height == 0)
            return AvaloniaColor.FromRgb(104, 248, 212);

        var buckets = new Dictionary<int, ColorBucket>();
        var sampleStep = Math.Max(1, Math.Min(bitmap.Width, bitmap.Height) / 180);

        for (var y = 0; y < bitmap.Height; y += sampleStep)
        {
            for (var x = 0; x < bitmap.Width; x += sampleStep)
            {
                var pixel = bitmap.GetPixel(x, y);
                if (pixel.Alpha < 160) continue;

                var max = Math.Max(pixel.Red, Math.Max(pixel.Green, pixel.Blue));
                var min = Math.Min(pixel.Red, Math.Min(pixel.Green, pixel.Blue));
                var saturation = max == 0 ? 0 : (max - min) / (float)max;

                // Near-black, near-white and grey pixels make poor control colors.
                if (max < 45 || min > 238 || saturation < 0.12f) continue;

                var key = ((pixel.Red >> 4) << 8) | ((pixel.Green >> 4) << 4) | (pixel.Blue >> 4);
                buckets.TryGetValue(key, out var bucket);
                buckets[key] = new ColorBucket(bucket.Count + 1,
                    bucket.Red + pixel.Red,
                    bucket.Green + pixel.Green,
                    bucket.Blue + pixel.Blue);
            }
        }

        if (buckets.Count == 0)
            return AvaloniaColor.FromRgb(104, 248, 212);

        var dominant = buckets.Values
            .OrderByDescending(bucket => bucket.Count)
            .First();

        var red = (byte)(dominant.Red / dominant.Count);
        var green = (byte)(dominant.Green / dominant.Count);
        var blue = (byte)(dominant.Blue / dominant.Count);

        return MakeReadable(red, green, blue);
    }

    private static AvaloniaColor MakeReadable(byte red, byte green, byte blue)
    {
        var max = Math.Max(red, Math.Max(green, blue));
        if (max < 150)
        {
            var scale = 150d / Math.Max(1, (int)max);
            red = (byte)Math.Min(255, red * scale);
            green = (byte)Math.Min(255, green * scale);
            blue = (byte)Math.Min(255, blue * scale);
        }

        return AvaloniaColor.FromRgb(red, green, blue);
    }
}
