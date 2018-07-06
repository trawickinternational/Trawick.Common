using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Trawick.Common.Extensions;

namespace Trawick.Common.Helpers
{
	public static class Common
	{

		public static string GetBrochurePath(int productId, int? agentId = null)
		{
			if (agentId != null)
			{
				return string.Format("/Document/ProductBrochure/{0}?AgentId={1}", productId, agentId);
			}
			return string.Format("/Document/ProductBrochure/{0}", productId);
		}




		private static string DEFAULT_BANNER = "/uploads/default/banner.jpg";


		public static string GetRandomBanner()
		{
			string path = @"/Content/images/banners/student";
			IEnumerable<FileInfo> imgFiles = GetImageFiles(path);
			int numFiles = imgFiles.Count();
			if (numFiles > 0)
			{
				int random = new Random().Next(0, numFiles);
				FileInfo file = imgFiles.ElementAt(random);
				return GetRelativePath(file);
			}
			return DEFAULT_BANNER;
		}


		public static string GetBannerPath(int id)
		{
			DirectoryInfo dir = GetSchoolDirectory(id);
			FileInfo file = dir.GetFiles("banner.jpg").FirstOrDefault();
			if (file == null)
			{
				//return DEFAULT_BANNER;
				return GetRandomBanner();
			}
			return GetRelativePath(file);
		}



		private static DirectoryInfo GetSchoolDirectory(int id)
		{
			string virtualPath = HostingEnvironment.ApplicationVirtualPath;

			string baseDir = HostingEnvironment.MapPath(string.Format("~/Uploads/{0}/", id));
			if (!Directory.Exists(baseDir))
			{
				//Directory.CreateDirectory(baseDir);
				baseDir = HostingEnvironment.MapPath("~/Uploads/Default/");
			}
			return new DirectoryInfo(baseDir);
		}



		private static IEnumerable<FileInfo> GetImageFiles(string path)
		{
			string[] extensions = new string[] { ".jpg", ".png" };
			string absPath = IsFullPath(path) ? path : HostingEnvironment.MapPath(path);
			DirectoryInfo dir = new DirectoryInfo(absPath);
			return (dir.GetFiles()).Where(f => extensions.Contains(f.Extension));
		}


		private static string GetRelativePath(FileInfo file)
		{
			Func<FileSystemInfo, string> getPath = fsi =>
			{
				var d = fsi as DirectoryInfo;
				return d == null ? fsi.FullName : d.FullName.TrimEnd('\\') + "\\";
			};

			var fromPath = HostingEnvironment.MapPath("~");
			var toPath = getPath(file);

			var fromUri = new Uri(fromPath);
			var toUri = new Uri(toPath);

			var relativeUri = fromUri.MakeRelativeUri(toUri);
			var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			return string.Format("/{0}", relativePath);
		}


		private static bool IsFullPath(string path)
		{
			try
			{
				return Path.GetFullPath(path) == path;
			}
			catch
			{
				return false;
			}
		}






		//private static IEnumerable<string> SearchAccessibleFiles(string root, string searchTerm)
		//{
		//	var files = new List<string>();
		//	foreach (var file in Directory.EnumerateFiles(root).Where(m => m.Contains(searchTerm)))
		//	{
		//		files.Add(file);
		//	}
		//	foreach (var subDir in Directory.EnumerateDirectories(root))
		//	{
		//		try
		//		{
		//			files.AddRange(SearchAccessibleFiles(subDir, searchTerm));
		//		}
		//		catch (UnauthorizedAccessException ex)
		//		{
		//			// ...
		//		}
		//	}
		//	return files;
		//}
		//// use example:
		//// var files = SearchAccesibleFiles(@"c:\", "bugs");
		//// will return every file on the c drive, in an accessible directory, with a name that contains 'bugs'



		

		public static Dictionary<string, string> BuildMapFrom<T>(T model)
		{
			//var map = new Dictionary<string, string>();
			//foreach (var prop in model.GetType().GetProperties())
			//{
			//	try
			//	{
			//		map.Add(prop.Name, prop.GetValue(model).ToString());
			//	}
			//	catch
			//	{

			//	}
			//}
			//return map;

			return model.GetPropertyMap();
		}






		public static void WriteBytesToStream(Stream stream, byte[] bytes)
		{
			using (var writer = new BinaryWriter(stream))
			{
				writer.Write(bytes);
			}
		}




		/// <summary>
		/// Converts a string to byte array
		/// </summary>
		/// <param name="input">The string</param>
		/// <returns>The byte array</returns>
		public static byte[] ConvertToByteArray(string input)
		{
			return input.Select(Convert.ToByte).ToArray();
		}

		/// <summary>
		/// Converts a byte array to a string
		/// </summary>
		/// <param name="bytes">The byte array</param>
		/// <returns>The string</returns>
		public static string ConvertToString(byte[] bytes)
		{
			return new string(bytes.Select(Convert.ToChar).ToArray());
		}

		/// <summary>
		/// Converts a byte array to a Base64 string
		/// </summary>
		/// <param name="bytes">The byte array</param>
		/// <returns>The Base64 string</returns>
		public static string ConvertToBase64String(byte[] bytes)
		{
			return Convert.ToBase64String(bytes);
		}





	}
}