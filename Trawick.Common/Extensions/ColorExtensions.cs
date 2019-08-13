using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Trawick.Common.Extensions
{
	public static class ColorExtensions
	{
		public static string ToHexString(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

		public static string ToRgbString(this Color c) => $"RGB({c.R}, {c.G}, {c.B})";



		// Coefficient (coef) is a value from 0 to 1; more is lighter, less is darker:
		public static Color ChangeLightness(this Color c, float coef)
		{
			return Color.FromArgb(c.A, (int)(c.R * coef), (int)(c.G * coef), (int)(c.B * coef));
		}

		// Or, if you'd like to use an integer value from 1 to 10 instead of the coefficient:
		public static Color ChangeLightness(this Color c, int lightness)
		{
			int MinLightness = 1;
			int MaxLightness = 10;
			float MinLightnessCoef = 1f;
			float MaxLightnessCoef = 0.4f;

			if (lightness < MinLightness)
				lightness = MinLightness;
			else if (lightness > MaxLightness)
				lightness = MaxLightness;

			float coef = MinLightnessCoef +
				(
					(lightness - MinLightness) *
						((MaxLightnessCoef - MinLightnessCoef) / (MaxLightness - MinLightness))
				);

			return Color.FromArgb(c.A, (int)(c.R * coef), (int)(c.G * coef), (int)(c.B * coef));
		}




		//public static Color GetContrastColor(this Color c)
		//{
		//	return Color.FromArgb(c.R > 127 ? 0 : 255, c.G > 127 ? 0 : 255, c.B > 127 ? 0 : 255);
		//}


		public static Color GetContrastColor(this Color c, string light = "ffffff", string dark = "000000")
		{
			var luma = c.GetLuma();
			//var brush = luma < 0.5 ? Brushes.White : Brushes.Black;
			//return new Pen(brush).Color;

			var contrast = luma < 0.5 ? light : dark;
			int argb = int.Parse(contrast.Replace("#", ""), NumberStyles.HexNumber);
			return Color.FromArgb(argb);
		}


		public static double GetLuma(this Color c)
		{
			float nR = c.R / 255.0f;
			float nG = c.G / 255.0f;
			float nB = c.B / 255.0f;
			return 0.2126 * nR + 0.7152 * nG + 0.0722 * nB;
		}


		public static String GetColorName(this Color c)
		{
			var predefined = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
			var match = (from p in predefined where ((Color)p.GetValue(null, null)).ToArgb() == c.ToArgb() select (Color)p.GetValue(null, null));
			if (match.Any())
				return match.First().Name;
			return String.Empty;
		}


		// https://www.cyotek.com/blog/finding-nearest-colors-using-euclidean-distance

		public static int GetDistance(this Color current, Color match)
		{
			int redDifference;
			int greenDifference;
			int blueDifference;

			redDifference = current.R - match.R;
			greenDifference = current.G - match.G;
			blueDifference = current.B - match.B;

			return redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;
		}

		public static int GetDistanceWithAlpha(this Color current, Color match)
		{
			int redDifference;
			int greenDifference;
			int blueDifference;
			int alphaDifference;

			alphaDifference = current.A - match.A;
			redDifference = current.R - match.R;
			greenDifference = current.G - match.G;
			blueDifference = current.B - match.B;

			return alphaDifference * alphaDifference + redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;
		}



		public static Color FindNearestColor(this Color current, List<Color> colors)
		{
			return FindNearestColor(current, colors.ToArray());
		}


		public static Color FindNearestColor(this Color current, Color[] colors)
		{
			int shortestDistance;
			int index;

			index = -1;
			shortestDistance = int.MaxValue;

			for (int i = 0; i < colors.Length; i++)
			{
				Color match;
				int distance;

				match = colors[i];
				distance = GetDistance(current, match);

				if (distance < shortestDistance)
				{
					index = i;
					shortestDistance = distance;
				}
			}

			return colors[index];
			//return index;
		}

		// https://www.cyotek.com/blog/an-introduction-to-dithering-images
		// https://stackoverflow.com/questions/28323448/fast-bitmap-modifying-using-bitmapdata-and-pointers-in-c-sharp

	}
}