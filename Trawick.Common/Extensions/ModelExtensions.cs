using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Extensions
{
	public static class ModelExtensions
	{
		/// <summary>
		/// Create a map of all public property names and their values.
		/// </summary>
		public static Dictionary<string, string> GetPropertyMap<T>(this T self)// where T : class
		{
			var map = new Dictionary<string, string>();
			foreach (var prop in self.GetType().GetProperties())
			{
				try
				{
					map.Add(prop.Name, prop.GetValue(self).ToString());
				}
				catch { }
			}
			return map;
		}



		public static bool TryUpdateProperty<TSource, TKey>(this TSource source, Expression<Func<TSource, TKey>> selector, TKey input)
		{
			if (!input.IsDefaultValue())
			{
				var expr = (MemberExpression)selector.Body;
				var prop = (PropertyInfo)expr.Member;
				prop.SetValue(source, input, null);
				return true;
			}
			return false;
		}
		// if (member.TryUpdateProperty(x => x.address1, "new address")) { }

	}
}