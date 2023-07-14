using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Nein.Extensions;

namespace OsuPlayer.Styles;

public class ColorSetter
{
    public static void SetColor(Color color)
    {
        var resourceProvider = (IResourceProvider) Application.Current!.Resources.MergedDictionaries.First();

        color = Color.FromArgb(255, color.R, color.G, color.B);

        //TODO: Check how to set values in the resource provider
        
        // resourceProvider.Loaded["SystemAccentColor"] = color;
        // resourceProvider.Loaded["SystemAccentColorDark1"] = CalculateShade(color, -0.1f);
        // resourceProvider.Loaded["SystemAccentColorDark2"] = CalculateShade(color, -0.2f);
        // resourceProvider.Loaded["SystemAccentColorDark3"] = CalculateShade(color, -0.3f);
        // resourceProvider.Loaded["SystemAccentColorLight1"] = CalculateShade(color, 0.1f);
        // resourceProvider.Loaded["SystemAccentColorLight2"] = CalculateShade(color, 0.2f);
        // resourceProvider.Loaded["SystemAccentColorLight3"] = CalculateShade(color, 0.3f);
    }

    private static Color CalculateShade(Color baseColor, float offset)
    {
        var color = System.Drawing.Color.FromArgb(baseColor.A, baseColor.R, baseColor.G, baseColor.B);

        var hsl = new HSL(color.GetHue(), color.GetSaturation(), color.GetPerceivedBrightness());

        hsl.L += offset;

        if (hsl.L > 1) hsl.L = 1;
        if (hsl.L < 0) hsl.L = 0;

        return hsl.ToColor();
    }

    private struct HSL
    {
        public float H { get; }
        public float S { get; }
        public float L { get; set; }

        public HSL(float h, float s, float l)
        {
            H = h;
            S = s;
            L = l;
        }

        public Color ToColor()
        {
            if (S == 0)
            {
                var l = (byte) (L * 255);
                return Color.FromArgb(255, l, l, l);
            }

            var h = H / 360f;

            var max = L < 0.5f ? L * (1 + S) : L + S - L * S;
            var min = L * 2f - max;

            return Color.FromArgb(255, (byte) (255 * HueToRgb(min, max, h + 1 / 3f)),
                (byte) (255 * HueToRgb(min, max, h)),
                (byte) (255 * HueToRgb(min, max, h - 1 / 3f)));
        }

        private static float HueToRgb(float v1, float v2, float vH)
        {
            if (vH < 0)
                vH += 1;

            if (vH > 1)
                vH -= 1;

            if (6 * vH < 1)
                return v1 + (v2 - v1) * 6 * vH;

            if (2 * vH < 1)
                return v2;

            if (3 * vH < 2)
                return v1 + (v2 - v1) * (2.0f / 3 - vH) * 6;

            return v1;
        }
    }
}