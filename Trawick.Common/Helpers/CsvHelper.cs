using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

        public static string ObjectToCsvData(IEnumerable<object> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj", "Value can not be null or Nothing!");
            }

            StringBuilder sb = new StringBuilder();
            foreach (var item in obj)
            {
                var sb2 = new StringBuilder();
                Type t = item.GetType();
                PropertyInfo[] pi = t.GetProperties();
                
                for (int index = 0; index < pi.Length; index++)
                {
                    if (pi[index].GetValue(item, null) != null)
                    {
                        sb2.Append(Csv.Escape(pi[index].GetValue(item, null).ToString()));

                    }
                    if (index < pi.Length - 1)
                    {
                        sb2.Append(",");
                    }
                }
                sb.AppendLine(sb2.ToString());
            }
            return sb.ToString();
        }

        public static class Csv
        {
            public static string Escape(string s)
            {
                if (s.Contains(QUOTE))
                    s = s.Replace(QUOTE, ESCAPED_QUOTE);

                if (s.IndexOfAny(CHARACTERS_THAT_MUST_BE_QUOTED) > -1)
                    s = QUOTE + s + QUOTE;

                return s;
            }

            public static string Unescape(string s)
            {
                if (s.StartsWith(QUOTE) && s.EndsWith(QUOTE))
                {
                    s = s.Substring(1, s.Length - 2);

                    if (s.Contains(ESCAPED_QUOTE))
                        s = s.Replace(ESCAPED_QUOTE, QUOTE);
                }

                return s;
            }


            private const string QUOTE = "\"";
            private const string ESCAPED_QUOTE = "\"\"";
            private static char[] CHARACTERS_THAT_MUST_BE_QUOTED = { ',', '"', '\n' };
        }
        
    }
}