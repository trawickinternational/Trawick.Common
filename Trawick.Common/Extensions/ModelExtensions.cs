using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Extensions
{
	public static class ModelExtensions
	{

		public static Dictionary<string, string> GetPropertyMap<T>(this T model)
		{
			var map = new Dictionary<string, string>();
			foreach (var prop in model.GetType().GetProperties())
			{
				try
				{
					map.Add(prop.Name, prop.GetValue(model).ToString());
				}
				catch { }
			}
			return map;
		}


	}
}
