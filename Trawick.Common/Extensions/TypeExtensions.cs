using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Extensions
{
	public static class TypeExtensions
	{
		public static object DefaultValue(this Type type)
		{
			if (type.GetTypeInfo().IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}

	}
}
