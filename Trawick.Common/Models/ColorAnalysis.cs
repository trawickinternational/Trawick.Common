using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Trawick.Common.Extensions;

namespace Trawick.Common.Models
{

	//public class Pixel
	//{
	//	public Color Color { get; set; }
	//	public Color WebColor { get; set; }

	//}


	public class ColorAnalysis
	{
		public Bitmap Bitmap;

		public List<Color> TenMostUsedColors { get; private set; }
		public List<int> TenMostUsedColorIncidences { get; private set; }

		public Color MostUsedColor { get; private set; }
		public int MostUsedColorIncidence { get; private set; }

		public Color PrimaryColor { get; private set; }
		public Color SecondaryColor { get; private set; }

		public bool HasTransparency { get; private set; }

		private bool ExcludeWhite;


		public Color LightestColor { get; private set; }
		public Color DarkestColor { get; private set; }



		#region Constructors


		public ColorAnalysis(string path, bool excludeWhite = false)
		{
			ExcludeWhite = excludeWhite;

			if (path.StartsWith("http"))
			{
				try
				{
					WebRequest request = WebRequest.Create(path);
					WebResponse response = request.GetResponse();
					Stream stream = response.GetResponseStream();
					Bitmap = new Bitmap(stream);
				}
				catch (Exception ex)
				{
					// "There was a problem downloading the file"
				}
			}
			else
			{
				Bitmap = Image.FromFile(path) as Bitmap;
			}
			//Bitmap = Image.FromFile(path) as Bitmap;
			AnalyzeColors();
		}


		public ColorAnalysis(Bitmap bitmap, bool excludeWhite = false)
		{
			ExcludeWhite = excludeWhite;
			Bitmap = bitmap;
			AnalyzeColors();
		}


		#endregion



		public Color PrimaryDarkerThan(string hex)
		{
			Color _color = ColorTranslator.FromHtml(hex);

			foreach (Color color in TenMostUsedColors)
			{
				if (color.GetBrightness() < _color.GetBrightness())
				{
					return color;
				}
			}
			return PrimaryColor;
		}


		#region Analyze


		private void AnalyzeColors()
		{
			TenMostUsedColors = new List<Color>();
			TenMostUsedColorIncidences = new List<int>();

			MostUsedColor = Color.Empty;
			MostUsedColorIncidence = 0;

			HasTransparency = Bitmap.HasTransparency();

			Color pixel;
			int pixelColor;
			var dctColorIncidence = new Dictionary<int, int>();

			// Added to help with gradients
			var dctWebColorIncidence = new Dictionary<int, int>();

			// this is what you want to speed up with unmanaged code
			for (int row = 0; row < Bitmap.Size.Width; row++)
			{
				for (int col = 0; col < Bitmap.Size.Height; col++)
				{

					// https://www.cyotek.com/blog/dithering-an-image-using-the-floyd-steinberg-algorithm-in-csharp
					// GetPixel, SetPixel: BAD

					pixel = Bitmap.GetPixel(row, col);
					// Don't allow transparent
					if (pixel.A == 255)
					{

						if (ExcludeWhite && pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
						{
							// This pixel is white. Exclude it?
							continue;
						}


						//if (ExcludeGray && !(pixel.R != pixel.G || pixel.G != pixel.B))
						//{
						//	// This pixel is gray. Exclude it?
						//	continue;
						//}



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
				Color _color = Color.FromArgb(kvp.Key);
				TenMostUsedColors.Add(_color);
				TenMostUsedColorIncidences.Add(kvp.Value);

				//TenMostUsedHexes.Add(_color.ToHexString());
				//TenMostUsedWebHexes.Add(GetNearestWebColor(_color).ToHexString());
			}

			MostUsedColor = Color.FromArgb(dctSortedByValueHighToLow.First().Key);
			MostUsedColorIncidence = dctSortedByValueHighToLow.First().Value;



			//var TenLightestColors = new List<Color>();
			//var TenDarkestColors = new List<Color>();

			List<Color> ColorsByBrightness = dctColorIncidence
				.Select(x => Color.FromArgb(x.Key))
				.OrderByDescending(x => x.GetBrightness()).ToList();

			LightestColor = ColorsByBrightness.First();
			DarkestColor = ColorsByBrightness.Last();


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


		#endregion



		private Color ChangeColorBrightness(Color color, float correctionFactor)
		{
			float red = (float)color.R;
			float green = (float)color.G;
			float blue = (float)color.B;

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


		#region Grayscale


		//private static unsafe bool IsGrayScale(Image image)
		//{
		//	using (var bmp = new Bitmap(image))
		//	{
		//		var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

		//		var pt = (int*)data.Scan0;
		//		var res = true;

		//		for (var i = 0; i < data.Height * data.Width; i++)
		//		{
		//			var color = Color.FromArgb(pt[i]);

		//			if (color.A != 0 && (color.R != color.G || color.G != color.B))
		//			{
		//				res = false;
		//				break;
		//			}
		//		}

		//		bmp.UnlockBits(data);

		//		return res;
		//	}
		//}


		//private bool IsGrayScale(Bitmap bitmap)
		//{
		//	bool res = true;
		//	unsafe
		//	{
		//		BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

		//		int bytesPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
		//		int heightInPixels = data.Height;
		//		int widthInBytes = data.Width * bytesPerPixel;
		//		byte* PtrFirstPixel = (byte*)data.Scan0;
		//		Parallel.For(0, heightInPixels, y =>
		//		{
		//			byte* currentLine = PtrFirstPixel + (y * data.Stride);
		//			for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
		//			{
		//				int b = currentLine[x];
		//				int g = currentLine[x + 1];
		//				int r = currentLine[x + 2];
		//				if (b != g || r != g)
		//				{
		//					res = false;
		//					break;
		//				}
		//			}
		//		});
		//		bitmap.UnlockBits(data);
		//	}
		//	return res;
		//}



		private Bitmap MakeGrayscale(Bitmap bitmap)
		{
			//Declare bmp as a new Bitmap with the same Width & Height
			Bitmap bmp = new Bitmap(bitmap.Width, bitmap.Height);

			for (int x = 0; x < bitmap.Width; x++)
			{
				for (int y = 0; y < bitmap.Height; y++)
				{
					//Get the Pixel
					Color pixel = bitmap.GetPixel(x, y);

					//Declare grayScale as the Grayscale Pixel
					int grayScale = (int)((pixel.R * 0.3) + (pixel.G * 0.59) + (pixel.B * 0.11));

					//Declare color as a Grayscale Color
					Color color = Color.FromArgb(grayScale, grayScale, grayScale);

					//Set the Grayscale Pixel
					bmp.SetPixel(x, y, color);
				}
			}
			return bmp;
		}


		#endregion

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