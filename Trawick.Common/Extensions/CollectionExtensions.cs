using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Trawick.Common.Extensions
{
	public static class CollectionExtensions
	{

		public static string ToQueryString(this NameValueCollection nvc, bool omitEmpty = false)
		{
			IEnumerable<string> nvcKeys = omitEmpty
				? nvc.AllKeys.Where(key => !string.IsNullOrWhiteSpace(nvc[key]))
				: nvc.AllKeys;

			var array = nvcKeys.Select(key => string.Format("{0}={1}",
				HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key])));

			return string.Format("?{0}", string.Join("&", array));
		}


		public static string ToQueryString(this Dictionary<string, string> parameters)
		{
			var array = parameters.Select(kvp => string.Format("{0}={1}",
				HttpUtility.UrlEncode(kvp.Key), HttpUtility.UrlEncode(kvp.Value)));
			return string.Format("?{0}", string.Join("&", array));
		}


		public static dynamic ToObject(this IDictionary<String, Object> dictionary)
		{
			var expandoObj = new ExpandoObject();
			var expandoObjCollection = (ICollection<KeyValuePair<String, Object>>)expandoObj;

			foreach (var keyValuePair in dictionary)
			{
				expandoObjCollection.Add(keyValuePair);
			}
			dynamic eoDynamic = expandoObj;
			return eoDynamic;
		}

		public static T ToObject<T>(this IDictionary<String, Object> dictionary) where T : class
		{
			return ToObject(dictionary) as T;
		}



		public static bool ContainsAny<T>(this IEnumerable<T> Collection, IEnumerable<T> Values)
		{
			return Collection.Any(m => Values.Contains(m));
		}

		public static bool ContainsAll<T>(this IEnumerable<T> Collection, IEnumerable<T> Values)
		{
			return Collection.All(m => Values.Contains(m));
		}



	}
}
