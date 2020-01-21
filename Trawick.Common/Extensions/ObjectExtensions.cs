using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Extensions
{
	public static class ObjectExtensions
	{
		public static string ToJsonString(this object obj)
		{
			var sb = new StringBuilder();
			using (var sw = new StringWriter(sb))
			{
				var js = new JsonSerializer();
				js.Serialize(sw, obj);
			}
			return sb.ToString();
		}

		public static bool IsDefaultValue(this object param)
		{
			return param == param.GetType().DefaultValue();
		}



		#region Property Copying

		// https://www.pluralsight.com/guides/property-copying-between-two-objects-using-reflection


		/// <summary>
		/// Copies similarly named public properties from the <paramref name="parent"/> object.
		/// </summary>
		public static void CopyPropertiesFrom(this object self, object parent)
		{
			foreach (var fromProp in parent.GetType().GetProperties())
			{
				foreach (var toProp in self.GetType().GetProperties())
				{
					if (fromProp.Name == toProp.Name && fromProp.PropertyType == toProp.PropertyType)
					{
						toProp.SetValue(self, fromProp.GetValue(parent));
						break;
					}
				}
			}
		}


		/// <summary>
		/// To be used with <see cref="MatchParentAttribute"/>.
		/// Copies public properties with matching attributes from the <paramref name="parent"/> object.
		/// </summary>
		public static void MatchPropertiesFrom(this object self, object parent)
		{
			var childProperties = self.GetType().GetProperties();
			foreach (var childProperty in childProperties)
			{
				var attributesForProperty = childProperty.GetCustomAttributes(typeof(MatchParentAttribute), true);
				var isOfTypeMatchParentAttribute = false;

				MatchParentAttribute currentAttribute = null;

				foreach (var attribute in attributesForProperty)
				{
					if (attribute.GetType() == typeof(MatchParentAttribute))
					{
						isOfTypeMatchParentAttribute = true;
						currentAttribute = (MatchParentAttribute)attribute;
						break;
					}
				}

				if (isOfTypeMatchParentAttribute)
				{
					var parentProperties = parent.GetType().GetProperties();
					object parentPropertyValue = null;
					foreach (var parentProperty in parentProperties)
					{
						if (parentProperty.Name == currentAttribute.ParentPropertyName)
						{
							if (parentProperty.PropertyType == childProperty.PropertyType)
							{
								parentPropertyValue = parentProperty.GetValue(parent);
							}
						}
					}
					childProperty.SetValue(self, parentPropertyValue);
				}
			}
		}

        #endregion

        public static Dictionary<string, string> ToDictionary(this NameValueCollection col)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var k in col.AllKeys)
            {
                dict.Add(k, col[k]);
            }
            return dict;
        }

    }


	/// <summary>
	/// To be used with <see cref="object.MatchPropertiesFrom"/>.
	/// Defines the matching property name in the parent object.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class MatchParentAttribute : Attribute
	{
		public readonly string ParentPropertyName;
		public MatchParentAttribute(string parentPropertyName)
		{
			ParentPropertyName = parentPropertyName;
		}
	}
    // [MatchParent("Name")]

   

}

