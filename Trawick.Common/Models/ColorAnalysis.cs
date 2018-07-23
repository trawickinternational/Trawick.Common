using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Models
{
	public class ColorAnalysis
	{
		public Bitmap bmp;

		public List<Color> TenMostUsedColors { get; private set; }
		public List<int> TenMostUsedColorIncidences { get; private set; }

		public Color MostUsedColor { get; private set; }
		public int MostUsedColorIncidence { get; private set; }

		public Color PrimaryColor { get; private set; }
		public Color SecondaryColor { get; private set; }


		public ColorAnalysis(string path)
		{
			bmp = Image.FromFile(path) as Bitmap;
			AnalyzeColors();
		}

		public ColorAnalysis(Bitmap bitmap)
		{
			bmp = bitmap;
			AnalyzeColors();
		}


		private void AnalyzeColors()
		{
			TenMostUsedColors = new List<Color>();
			TenMostUsedColorIncidences = new List<int>();

			MostUsedColor = Color.Empty;
			MostUsedColorIncidence = 0;


			Color pixel;
			int pixelColor;
			var dctColorIncidence = new Dictionary<int, int>();

			// Added to help with gradients
			var dctWebColorIncidence = new Dictionary<int, int>();

			// this is what you want to speed up with unmanaged code
			for (int row = 0; row < bmp.Size.Width; row++)
			{
				for (int col = 0; col < bmp.Size.Height; col++)
				{

					// https://www.cyotek.com/blog/dithering-an-image-using-the-floyd-steinberg-algorithm-in-csharp
					// GetPixel, SetPixel: BAD

					pixel = bmp.GetPixel(row, col);
					// Don't allow transparent
					if (pixel.A == 255)
					{
						pixelColor = pixel.ToArgb();
						if (dctColorIncidence.Keys.Contains(pixelColor))
						{
							dctColorIncidence[pixelColor]++;
						}
						else
						{
							dctColorIncidence.Add(pixelColor, 1);
						}

						// Added to help with gradients
						pixelColor = GetNearestWebColor(pixel).ToArgb();
						if (dctWebColorIncidence.Keys.Contains(pixelColor))
						{
							dctWebColorIncidence[pixelColor]++;
						}
						else
						{
							dctWebColorIncidence.Add(pixelColor, 1);
						}
					}
				}
			}


			//var TenMostUsedHexes = new List<string>();
			//var TenMostUsedWebHexes = new List<string>();


			var dctSortedByValueHighToLow = dctColorIncidence
				.OrderByDescending(x => x.Value)
				.ToDictionary(x => x.Key, x => x.Value);

			foreach (KeyValuePair<int, int> kvp in dctSortedByValueHighToLow.Take(10))
			{
				TenMostUsedColors.Add(Color.FromArgb(kvp.Key));
				TenMostUsedColorIncidences.Add(kvp.Value);

				//TenMostUsedHexes.Add(Color.FromArgb(kvp.Key).ToHexString());
				//TenMostUsedWebHexes.Add(GetNearestWebColor(Color.FromArgb(kvp.Key)).ToHexString());
			}

			MostUsedColor = Color.FromArgb(dctSortedByValueHighToLow.First().Key);
			MostUsedColorIncidence = dctSortedByValueHighToLow.First().Value;


			// Added to help with gradients
			//var MostUsedWebColors = new List<Color>();
			//var dctWebSortedByValueHighToLow = dctWebColorIncidence
			//	.OrderByDescending(x => x.Value)
			//	.ToDictionary(x => x.Key, x => x.Value);

			//foreach (KeyValuePair<int, int> kvp in dctWebSortedByValueHighToLow.Take(10))
			//{
			//	MostUsedWebColors.Add(Color.FromArgb(kvp.Key));
			//}

			Color MostUsedWebColor = Color.FromArgb(dctWebColorIncidence
				.OrderByDescending(x => x.Value)
				.ToDictionary(x => x.Key, x => x.Value)
				.First().Key);

			MostUsedWebColor = GetNearestWebColor(MostUsedWebColor);

			//var MostUsedWebHex = MostUsedWebColor.ToHexString();

			foreach (Color color in TenMostUsedColors)
			{
				//var name = color.GetColorName();
				//var webName = MostUsedWebColor.GetColorName();

				//var nearest = GetNearestWebColor(color);
				//var nearestName = nearest.GetColorName();

				//var nearestWeb = GetNearestWebColor(MostUsedWebColor);
				//var nearestWebName = nearestWeb.GetColorName();

				if (GetNearestWebColor(color) == MostUsedWebColor)
				{
					if (PrimaryColor.IsEmpty)
					{
						PrimaryColor = color;
					}
				}
				else
				{
					if (SecondaryColor.IsEmpty)
					{
						SecondaryColor = color;
					}
				}
			}

			if (PrimaryColor.IsEmpty)
			{
				PrimaryColor = MostUsedColor;
			}
			if (SecondaryColor.IsEmpty)
			{
				SecondaryColor = PrimaryColor;
			}

		}



		private List<Color> WebColors =
			Enum.GetValues(typeof(KnownColor))
			.Cast<KnownColor>()
			.Select(kc => Color.FromKnownColor(kc))
			.OrderBy(c => c.GetHue()).ToList();


		private Color GetNearestWebColor(Color color)
		{

			double distance = 500.0;

			double red = Convert.ToDouble(color.R);
			double green = Convert.ToDouble(color.G);
			double blue = Convert.ToDouble(color.B);

			Color NearestColor = Color.Empty;

			foreach (Color col in WebColors)
			{
				// compute the Euclidean distance between the two colors
				// note, that the alpha-component is not used in this example
				double r = Math.Pow(Convert.ToDouble(col.R) - red, 2.0);
				double g = Math.Pow(Convert.ToDouble(col.G) - green, 2.0);
				double b = Math.Pow(Convert.ToDouble(col.B) - blue, 2.0);

				// it is not necessary to compute the square root
				// it should be sufficient to use:
				// temp = b + g + r;
				// if you plan to do so, the distance should be initialized by 250000.0
				var temp = Math.Sqrt(b + g + r);

				// explore the result and store the nearest color
				if (temp == 0.0)
				{
					// the lowest possible distance is - of course - zero
					// so I can break the loop (thanks to Willie Deutschmann)
					// here I could return the input_color itself
					// but in this example I am using a list with named colors
					// and I want to return the Name-property too
					NearestColor = col;
					break;
				}
				else if (temp < distance)
				{
					distance = temp;
					NearestColor = col;
				}
			}

			return NearestColor;
		}

	}
}

// https://www.codeproject.com/Questions/677506/Csharp-find-the-majority-color-of-an-image


#region OLD


//namespace Trawick.Common.Helpers
//{
//	public static class ColorMath
//	{

//		public static Color GetDominantColor(string path)
//		{
//			return GetDominantColor(Bitmap.FromFile(path) as Bitmap);
//		}


//		public static Color GetDominantColor(Bitmap bmp)
//		{
//			var TenMostUsedColors = new List<Color>();
//			var TenMostUsedColorIncidences = new List<int>();

//			var MostUsedColor = Color.Empty;
//			var MostUsedColorIncidence = 0;

//			int pixelColor;

//			var dctColorIncidence = new Dictionary<int, int>();

//			// this is what you want to speed up with unmanaged code
//			for (int row = 0; row < bmp.Size.Width; row++)
//			{
//				for (int col = 0; col < bmp.Size.Height; col++)
//				{
//					pixelColor = bmp.GetPixel(row, col).ToArgb();

//					if (dctColorIncidence.Keys.Contains(pixelColor))
//					{
//						dctColorIncidence[pixelColor]++;
//					}
//					else
//					{
//						dctColorIncidence.Add(pixelColor, 1);
//					}
//				}
//			}

//			// note that there are those who argue that a
//			// .NET Generic Dictionary is never guaranteed
//			// to be sorted by methods like this
//			var dctSortedByValueHighToLow = dctColorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

//			// this should be replaced with some elegant Linq ?
//			foreach (KeyValuePair<int, int> kvp in dctSortedByValueHighToLow.Take(10))
//			{
//				TenMostUsedColors.Add(Color.FromArgb(kvp.Key));
//				TenMostUsedColorIncidences.Add(kvp.Value);
//			}

//			MostUsedColor = Color.FromArgb(dctSortedByValueHighToLow.First().Key);
//			MostUsedColorIncidence = dctSortedByValueHighToLow.First().Value;

//			return MostUsedColor;
//		}

//	}
//}

#endregion