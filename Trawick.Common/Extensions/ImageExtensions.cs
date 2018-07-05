using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
				image.Save(ms, image.RawFormat);
				return ms.ToArray();
			}
		}

		public static string ToDataURL(this Image image)
		{
			string dataStr = Convert.ToBase64String(image.ToByteArray());
			return string.Format("data:{0};base64,{1}", image.GetMimeType(), dataStr);
		}

	}
}