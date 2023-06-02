using Avalonia.Media.Imaging;
using SkiaSharp;

namespace OsuPlayer.Modules;

public static class BitmapExtensions
{
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
}