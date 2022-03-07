using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Skia;
using Avalonia.Visuals.Media.Imaging;
using SkiaSharp;

namespace OsuPlayer.Controls;

public class SkiaImage : Control
{
    private SKPaint skPaint;
    private RenderTargetBitmap renderTarget;
    private ISkiaDrawingContextImpl skiaContext;

    public static readonly StyledProperty<SKBitmap> SourceProperty =
        AvaloniaProperty.Register<Image, SKBitmap>(nameof(Source));

    public static readonly StyledProperty<string> UriSourceProperty =
        AvaloniaProperty.Register<Image, string>(nameof(UriSource));

    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<Image, Stretch>(nameof(Stretch), Stretch.UniformToFill);

    public static readonly StyledProperty<float> BlurStrengthProperty =
        AvaloniaProperty.Register<Image, float>(nameof(BlurStrength), 0F);

    public static readonly StyledProperty<BitmapInterpolationMode> BitMapInterpolationModeProperty =
        AvaloniaProperty.Register<Image, BitmapInterpolationMode>(nameof(BitmapInterpolationMode));

    public SKBitmap Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public string UriSource
    {
        get => GetValue(UriSourceProperty);
        set
        {
            SetValue(UriSourceProperty, value);
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var uri = new Uri(value, UriKind.Absolute);
            if (assets!.Exists(uri))
            {
                var stream = assets.Open(uri);
                SetValue(SourceProperty, SKBitmap.Decode(stream));
            }
        }
    }

    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    public float BlurStrength
    {
        get => GetValue(BlurStrengthProperty);
        set => SetValue(BlurStrengthProperty, value);
    }

    public BitmapInterpolationMode BitmapInterpolationMode
    {
        get => GetValue(BitMapInterpolationModeProperty);
        set => SetValue(BitMapInterpolationModeProperty, value);
    }
    
    static SkiaImage()
    {
    }

    public override void EndInit()
    {
        base.EndInit();
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    {
        if (Source != null)
        {
            if (change.Property == SourceProperty)
            {
                skPaint = new SKPaint();
                skPaint.FilterQuality = SKFilterQuality.High;
                skPaint.IsAntialias = true;
                skPaint.ImageFilter = SKImageFilter.CreateBlur(BlurStrength, BlurStrength);

                renderTarget = new RenderTargetBitmap(new PixelSize(Source.Width, Source.Height),
                    new Vector(96, 96));

                var skContext = renderTarget.CreateDrawingContext(null);
                skiaContext = (ISkiaDrawingContextImpl) skContext;
                skiaContext.SkCanvas.Clear(new SKColor(255, 255, 255, 0));
                skiaContext.SkCanvas.DrawBitmap(Source, 0, 0, skPaint);
                InvalidateVisual();
            }
            else if (change.Property == BlurStrengthProperty)
            {
                skPaint.ImageFilter = SKImageFilter.CreateBlur(BlurStrength, BlurStrength);
                skiaContext.SkCanvas.Clear(new SKColor(255, 255, 255));
                skiaContext.SkCanvas.DrawBitmap(Source, 0, 0, skPaint);
                InvalidateVisual();
            }
        }

        base.OnPropertyChanged(change);
    }

    public override void Render(DrawingContext context)
    {
        if (Source != null)
        {
            var srcSize = new Size(Source.Width, Source.Height);
            var scale = Stretch.CalculateScaling(Bounds.Size, srcSize);
            var scaledSize = srcSize * scale;
            var destRect = new Rect(Bounds.Size).CenterRect(new Rect(scaledSize)).Intersect(new Rect(Bounds.Size));
            var sourceRect = new Rect(srcSize).CenterRect(new Rect(destRect.Size / scale));
            context.DrawImage(renderTarget, sourceRect, destRect, BitmapInterpolationMode);
        }
    }
}