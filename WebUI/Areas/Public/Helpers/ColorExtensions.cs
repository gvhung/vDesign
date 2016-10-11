using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebUI.Areas.Public.Helpers
{
    public class ColorExtensions
    {
        public static Color GetColor(string color)
        {
            Color c;

            if (color.IndexOf("RGB", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                var colors = Regex.Matches(color, @"[+-]?\d+(\.\d+)?").Cast<Match>()
                    .Select(x => int.Parse(x.Value)).ToArray();

                c = Color.FromArgb(colors[3], colors[0], colors[1], colors[2]);
            }
            else
            {
                c = ColorTranslator.FromHtml(color);
            }

            return c;
        }

        public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            var red = (float)color.R;
            var green = (float)color.G;
            var blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }
    }


}