using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Models
{
    public class ColorBlender
    {

        public static List<Color> StepsBetween(string color1, string color2, int steps)
        {
            var col1 = ColorTranslator.FromHtml(color1);
            var col2 = ColorTranslator.FromHtml(color2);
            return StepsBetween(col1, col2, steps);
        }

        public static List<Color> StepsBetween(Color color1, Color color2, int steps)
        {
            var count = steps + 1;
            var redStep = (double)(color2.R - color1.R) / count;
            var greenStep = (double)(color2.G - color1.G) / count;
            var blueStep = (double)(color2.B - color1.B) / count;

            var colors = new List<Color>
            {
                color1
            };

            for (int i = 1; i < count; i++)
            {
                var red = (int)Math.Round(color1.R + (i * redStep));
                var green = (int)Math.Round(color1.G + (i * greenStep));
                var blue = (int)Math.Round(color1.B + (i * blueStep));
                colors.Add(Color.FromArgb(255, red, green, blue));
            }

            colors.Add(color2);
            return colors;
        }




    }
}
