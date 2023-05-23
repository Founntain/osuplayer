using Avalonia.Media.Imaging;
using SkiaSharp;

namespace OsuPlayer.Modules;

public static class BitmapExtensions
{
    public static Bitmap BlurBitmap(this Bitmap bitmap, float blurRadius = 10f, float opacity = 1f)
    {
        SKBitmap originalBitmap = null;
        var blurSigma = blurRadius / 2;

        using (var stream = new MemoryStream())
        {
            bitmap.Save(stream);

            stream.Position = 0;
            
            originalBitmap = SKBitmap.Decode(stream);
        }

        var originalImage = SKImage.FromBitmap(originalBitmap);
        
        var blurFilter = SKImageFilter.CreateBlur(blurRadius, blurSigma);
        
        // Create the paint with the blur filter
        var paint = new SKPaint();
        paint.ImageFilter = blurFilter;
        
        paint.Color = new SKColor(alpha: (byte)(255 * opacity), red: 0, green: 0, blue: 0); // Set the opacity using the Color property
        
        // Create the bitmap for the blurred image
        var blurredBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);
        
        // Draw the original image with the blur effect onto the blurred bitmap
        using (var canvas = new SKCanvas(blurredBitmap))
        {
            // canvas.Clear(SKColors.Black);
            
            canvas.DrawImage(originalImage, 0, 0, paint);
        }

        Bitmap blurredAvaloniaBitmap = null;
        
        // Convert the SkiaSharp SKBitmap back to an Avalonia Bitmap
        using (var stream = new MemoryStream())
        {
            blurredBitmap.Encode(stream, SKEncodedImageFormat.Png, 80);

            stream.Position = 0;
            
            blurredAvaloniaBitmap = new Bitmap(stream);
        }

        return blurredAvaloniaBitmap;
    }
    
    public static Bitmap BlurBitmap(string imagePath, float blurRadius = 10f, float opacity = 1f)
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

        image.Encode(SKEncodedImageFormat.Png, 80).SaveTo(outputStream);
        outputStream.Position = 0;

        return new Bitmap(outputStream);
    }
}