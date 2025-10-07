using System.Windows.Media;

namespace Stats_Mafia.Utilities
{
    internal static class ColourHelper
    {
        public static (byte, byte, byte) HSVToRGB(float hue, float saturation, float value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            float f = hue / 60 - (float)Math.Floor(hue / 60);
            value = value * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - saturation));
            byte q = Convert.ToByte(value * (1 - f * saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));
            if (hi == 0)
                return (v, t, p);
            else if (hi == 1)
                return (q, v, p);
            else if (hi == 2)
                return (p, v, t);
            else if (hi == 3)
                return (p, q, v);
            else if (hi == 4)
                return (t, p, v);
            else
                return(v, p, q);
        }

        public static Color HSVToRGB(this Color col, float hue, float saturation, float value)
        {
            (byte r, byte g, byte b) = HSVToRGB(hue, saturation, value);
            return Color.FromRgb(r, g, b);
        }

        public static Brush HSVToRGB(this Brush brush, float hue, float saturation, float value)
        {
            // C# 14 allows static extension methods, but we are using C# 13, so we have to create a 'Color' object to be able to access its extension method
            Color col = new Color().HSVToRGB(hue, saturation, value);
            return new SolidColorBrush(col);

        }
    }
}
