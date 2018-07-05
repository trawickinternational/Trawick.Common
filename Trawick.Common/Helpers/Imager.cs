using Trawick.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace Trawick.Common.Helpers
{
	public static class Imager
	{

		/// <summary>Save image as jpeg</summary>
		/// <param name="path">path where to save</param>
		/// <param name="img">image to save</param>
		public static void SaveJpeg(string path, Image img)
		{
			var qualityParam = new EncoderParameter(Encoder.Quality, 100L);
			var jpegCodec = GetEncoderInfo("image/jpeg");
			var encoderParams = new EncoderParameters(1);
			encoderParams.Param[0] = qualityParam;
			img.Save(path, jpegCodec, encoderParams);
		}


		/// <summary>Save image</summary>
		/// <param name="path">path where to save</param>
		/// <param name="img">image to save</param>
		/// <param name="imageCodecInfo">codec info</param>
		public static void Save(string path, Image img, ImageCodecInfo imageCodecInfo)
		{
			var qualityParam = new EncoderParameter(Encoder.Quality, 100L);
			var encoderParams = new EncoderParameters(1);
			encoderParams.Param[0] = qualityParam;
			img.Save(path, imageCodecInfo, encoderParams);
		}


		/// <summary>Get codec info by mime type</summary>
		/// <param name="mimeType"></param>
		/// <returns></returns>
		public static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			return ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == mimeType);
		}


		/// <summary>Image remains the same size, and it is placed in the middle of the new canvas</summary>
		/// <param name="image">image to put on canvas</param>
		/// <param name="width">canvas width</param>
		/// <param name="height">canvas height</param>
		/// <param name="canvasColor">canvas color</param>
		/// <returns></returns>
		public static Image PutOnCanvas(Image image, int width, int height, Color canvasColor)
		{
			var res = new Bitmap(width, height);
			using (var g = Graphics.FromImage(res))
			{
				g.Clear(canvasColor);
				var x = (width - image.Width) / 2;
				var y = (height - image.Height) / 2;
				g.DrawImageUnscaled(image, x, y, image.Width, image.Height);
			}
			return res;
		}


		/// <summary>Image remains the same size, and it is placed in the middle of the new canvas</summary>
		/// <param name="image">image to put on canvas</param>
		/// <param name="width">canvas width</param>
		/// <param name="height">canvas height</param>
		/// <returns></returns>
		public static Image PutOnWhiteCanvas(Image image, int width, int height)
		{
			return PutOnCanvas(image, width, height, Color.White);
		}


		/// <summary>Resize an image and maintain aspect ratio</summary>
		/// <param name="image">image to be resized</param>
		/// <param name="newWidth">desired width</param>
		/// <param name="maxHeight">max height</param>
		/// <param name="onlyResizeIfWider">if image width is smaller than newWidth use image width</param>
		/// <returns>resized image</returns>
		public static Image Resize(Image image, int newWidth, int maxHeight, bool onlyResizeIfWider)
		{
			if (onlyResizeIfWider && image.Width <= newWidth) newWidth = image.Width;
			var newHeight = image.Height * newWidth / image.Width;
			if (newHeight > maxHeight)
			{
				// Resize with height instead
				newWidth = image.Width * maxHeight / image.Height;
				newHeight = maxHeight;
			}
			var res = new Bitmap(newWidth, newHeight);
			using (var graphic = Graphics.FromImage(res))
			{
				graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphic.SmoothingMode = SmoothingMode.HighQuality;
				graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphic.CompositingQuality = CompositingQuality.HighQuality;
				graphic.DrawImage(image, 0, 0, newWidth, newHeight);
			}
			return res;
		}


		/// <summary>
		/// Crop an image according to a selection rectangle
		/// </summary>
		/// <param name="img">image to be cropped</param>
		/// <param name="cropArea">rectangle to crop</param>
		/// <returns>cropped image</returns>
		public static Image Crop(Image img, Rectangle cropArea)
		{
			var bmpImage = new Bitmap(img);
			var bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
			return bmpCrop;
		}


		public static byte[] imageToByteArray(Image imageIn)
		{
			MemoryStream ms = new MemoryStream();
			imageIn.Save(ms, ImageFormat.Gif);
			return ms.ToArray();
		}

		public static Image byteArrayToImage(byte[] byteArrayIn)
		{
			MemoryStream ms = new MemoryStream(byteArrayIn);
			Image returnImage = Image.FromStream(ms);
			return returnImage;
		}

		// The actual converting function
		public static string GetImage(object img)
		{
			return "data:image/jpg;base64," + Convert.ToBase64String((byte[])img);
		}


		public static void PerformImageResizeAndPutOnCanvas(string pFilePath, string pFileName, int pWidth, int pHeight, string pOutputFileName)
		{
			Image imgBef = Image.FromFile(pFilePath + pFileName);
			Image _imgR = Imager.Resize(imgBef, pWidth, pHeight, true);
			Image _img2 = Imager.PutOnCanvas(_imgR, pWidth, pHeight, Color.White);
			//Save JPEG
			Imager.SaveJpeg(pFilePath + pOutputFileName, _img2);
		}
	}
}
// https://www.c-sharpcorner.com/article/resize-image-in-c-sharp/







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






namespace Trawick.Common.Helpers
{
	public class ColorAnalysis
	{
		public Bitmap bmp;

		private int pixelColor;
		private Dictionary<int, int> dctColorIncidence;

		public List<Color> TenMostUsedColors { get; private set; }
		public List<int> TenMostUsedColorIncidences { get; private set; }

		public Color MostUsedColor { get; private set; }
		public int MostUsedColorIncidence { get; private set; }


		public ColorAnalysis(string path)
		{
			this.bmp = Image.FromFile(path) as Bitmap;
			AnalyzeColors();
		}

		public ColorAnalysis(Bitmap bmp)
		{
			this.bmp = bmp;
			AnalyzeColors();
		}


		private void AnalyzeColors()
		{
			TenMostUsedColors = new List<Color>();
			TenMostUsedColorIncidences = new List<int>();


			MostUsedColor = Color.Empty;
			MostUsedColorIncidence = 0;

			dctColorIncidence = new Dictionary<int, int>();

			// this is what you want to speed up with unmanaged code
			for (int row = 0; row < bmp.Size.Width; row++)
			{
				for (int col = 0; col < bmp.Size.Height; col++)
				{
					pixelColor = bmp.GetPixel(row, col).ToArgb();
					if (dctColorIncidence.Keys.Contains(pixelColor))
					{
						dctColorIncidence[pixelColor]++;
					}
					else
					{
						dctColorIncidence.Add(pixelColor, 1);
					}
				}
			}

			var dctSortedByValueHighToLow = dctColorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

			foreach (KeyValuePair<int, int> kvp in dctSortedByValueHighToLow.Take(10))
			{
				TenMostUsedColors.Add(Color.FromArgb(kvp.Key));
				TenMostUsedColorIncidences.Add(kvp.Value);
			}

			MostUsedColor = Color.FromArgb(dctSortedByValueHighToLow.First().Key);
			MostUsedColorIncidence = dctSortedByValueHighToLow.First().Value;
		}
	}
}
// https://www.codeproject.com/Questions/677506/Csharp-find-the-majority-color-of-an-image