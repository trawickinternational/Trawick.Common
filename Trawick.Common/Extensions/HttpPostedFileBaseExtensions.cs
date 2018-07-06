using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Trawick.Common.Extensions
{
	public static class HttpPostedFileBaseExtensions
	{

		public static byte[] ToByteArray(this HttpPostedFileBase file)
		{
			using (var br = new BinaryReader(file.InputStream))
			{
				return br.ReadBytes(file.ContentLength);
			}
		}

		public static string ToDataURL(this HttpPostedFileBase file)
		{
			string dataStr = Convert.ToBase64String(file.ToByteArray());
			return string.Format("data:{0};base64,{1}", file.ContentType, dataStr);
		}

		public static bool IsImage(this HttpPostedFileBase file)
		{
			if (file.ContentType.Contains("image"))
			{
				return true;
			}
			string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };
			return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
		}

	}
}