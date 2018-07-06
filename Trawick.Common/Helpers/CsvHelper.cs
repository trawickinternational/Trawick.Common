using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Trawick.Common.Helpers
{
	public static class CsvHelper
	{

		public static List<Dictionary<string, string>> GetAsDictionary(string filePath)
		{
			return GetAsDictionary(FileToByteArray(filePath));
		}


		public static List<Dictionary<string, string>> GetAsDictionary(byte[] bytes)
		{
			var result = new List<Dictionary<string, string>>();

			using (var fs = new MemoryStream(bytes))
			{
				using (var file = new StreamReader(fs))
				{
					string line;
					int n = 0;
					List<string> columns = null;
					while ((line = file.ReadLine()) != null)
					{
						var values = SplitCsv(line);
						if (n == 0)
						{
							columns = values;
						}
						else
						{
							var dict = new Dictionary<string, string>();
							for (int i = 0; i < columns.Count; i++)
								if (i < values.Count)
									dict.Add(columns[i], values[i]);
							result.Add(dict);
						}
						n++;
					}
				}
			}
			return result;
		}


		//public List<Dictionary<string, string>> LoadCsvAsDictionary(string filePath)
		//{
		//	var result = new List<Dictionary<string, string>>();

		//	using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		//	{
		//		using (var file = new StreamReader(fs))
		//		{
		//			string line;
		//			int n = 0;
		//			List<string> columns = null;
		//			while ((line = file.ReadLine()) != null)
		//			{
		//				var values = SplitCsv(line);
		//				if (n == 0)
		//				{
		//					columns = values;
		//				}
		//				else
		//				{
		//					var dict = new Dictionary<string, string>();
		//					for (int i = 0; i < columns.Count; i++)
		//						if (i < values.Count)
		//							dict.Add(columns[i], values[i]);
		//					result.Add(dict);
		//				}
		//				n++;
		//			}
		//		}
		//	}
		//	return result;
		//}


		private static List<string> SplitCsv(string csv)
		{
			var values = new List<string>();

			int last = -1;
			bool inQuotes = false;

			int n = 0;
			while (n < csv.Length)
			{
				switch (csv[n])
				{
					case '"':
						inQuotes = !inQuotes;
						break;
					case ',':
						if (!inQuotes)
						{
							values.Add(csv.Substring(last + 1, (n - last)).Trim(' ', ','));
							last = n;
						}
						break;
				}
				n++;
			}

			if (last != csv.Length - 1)
				values.Add(csv.Substring(last + 1).Trim());

			return values;
		}


		private static byte[] FileToByteArray(string filePath)
		{
			if (File.Exists(filePath))
			{
				return File.ReadAllBytes(filePath);
			}
			return null;
		}


	}
}