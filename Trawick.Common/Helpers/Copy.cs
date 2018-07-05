﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trawick.Common.Helpers
{
	public class Copy
	{
		public static void Work<T>(IList<T> targetList, IList<T> sourceList, Func<T, int> selector)
		{
			var matchingPrimaryKey = targetList.Select(x => selector(x)).ToList();

			foreach (var thismatchingPrimaryKey in matchingPrimaryKey)
			{
				CopyValues<T>(targetList.Single(x => selector(x) == thismatchingPrimaryKey),
						sourceList.Single(x => selector(x) == thismatchingPrimaryKey));
			}
		}

		public static T CopyValues<T>(T target, T source)
		{
			Type t = typeof(T);

			var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

			foreach (var prop in properties)
			{
				var value = prop.GetValue(source, null);
				if (value != null)
					prop.SetValue(target, value, null);
			}

			return target;
		}

	}
}

// https://stackoverflow.com/questions/8702603/merging-two-objects-in-c-sharp