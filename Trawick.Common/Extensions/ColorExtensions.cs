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

		public static int FindNearestColor(this Color current, Color[] map)
		{
			int shortestDistance;
			int index;

			index = -1;
			shortestDistance = int.MaxValue;

			for (int i = 0; i < map.Length; i++)
			{
				Color match;
				int distance;

				match = map[i];
				distance = GetDistance(current, match);

				if (distance < shortestDistance)
				{
					index = i;
					shortestDistance = distance;
				}
			}
			return index;
		}


		// https://www.cyotek.com/blog/an-introduction-to-dithering-images
		// https://stackoverflow.com/questions/28323448/fast-bitmap-modifying-using-bitmapdata-and-pointers-in-c-sharp

	}
}