using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Trawick.Common.Extensions
{
	public static class ImageExtensions
	{

		public static string GetMimeType(this ImageFormat imageFormat)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			return codecs.First(codec => codec.FormatID == imageFormat.Guid).MimeType;
		}

		public static string GetMimeType(this Image image)
		{
			return image.RawFormat.GetMimeType();
		}

		public static byte[] ToByteArray(this Image image)
		{
			using (var ms = new MemoryStream())
			{
				image.Save(ms, ImageFormat.Bmp);
				return ms.ToArray();
			}
		}


        public static byte[] ToByteArray(this Stream input)
{
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static string ToDataURL(this Image image)
		{
			string dataStr = Convert.ToBase64String(image.ToByteArray());
			return string.Format("data:{0};base64,{1}", image.GetMimeType(), dataStr);
		}



		#region IsImage


		public static bool IsValidImage(this string filename)
		{
			try
			{
				using (Image newImage = Image.FromFile(filename))
				{ }
			}
			catch (OutOfMemoryException ex)
			{
				//The file does not have a valid image format.
				//-or- GDI+ does not support the pixel format of the file
				return false;
			}
			return true;
		}


		public static bool IsImageExtension(this string ext)
		{
			if (ext.Contains('.'))
			{
				ext = Path.GetExtension(ext);
			}
			return ImageFileExtensions.Contains(ext.ToLower());
		}


		private static List<string> ImageFileExtensions
		{
			get
			{
				return ImageCodecInfo.GetImageEncoders()
					.Select(c => c.FilenameExtension)
					.SelectMany(e => e.Split(';'))
					.Select(e => e.Replace("*", "").ToLower())
					.ToList();
			}
		}

		#endregion


		#region HasTransparency


		public static bool HasTransparency(this Image image)
		{
			var bitmap = image as Bitmap;
			if (bitmap != null)
			{
				return HasTransparency(bitmap);
			}
			using (bitmap = new Bitmap(image))
			{
				return HasTransparency(bitmap);
			}
		}

        public static bool HasTransparency(this Bitmap bitmap)
        {
            if (bitmap != null)
            { 
                // Not an alpha-capable color format. Note that GDI+ indexed images are alpha-capable on the palette.
                if (((ImageFlags)bitmap.Flags & ImageFlags.HasAlpha) == 0)
                    return false;
            // Indexed format, and no alpha colours in the images palette: immediate pass.
            if ((bitmap.PixelFormat & PixelFormat.Indexed) != 0 && bitmap.Palette.Entries.All(c => c.A == 255))
                return false;
            // Get the byte data 'as 32-bit ARGB'. This offers a converted version of the image data without modifying the original image.
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Int32 len = bitmap.Height * data.Stride;
            Byte[] bytes = new Byte[len];
            Marshal.Copy(data.Scan0, bytes, 0, len);
            bitmap.UnlockBits(data);
            // Check the alpha bytes in the data. Since the data is little-endian, the actual byte order is [BB GG RR AA]
            for (Int32 i = 3; i < len; i += 4)
                if (bytes[i] != 255)
                    return true;
        }
			return false;
		}

		#endregion

	}
}

// https://stackoverflow.com/questions/3064854/determine-if-alpha-channel-is-used-in-an-image
// https://stackoverflow.com/questions/670546/determine-if-file-is-an-image