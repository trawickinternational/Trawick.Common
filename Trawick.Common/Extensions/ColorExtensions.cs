using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
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

	}
}