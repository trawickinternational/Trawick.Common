using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Extensions
{
	public static class DataReaderExtension
	{
		public static IEnumerable<Dictionary<string, object>> AsEnumerable(this System.Data.IDataReader source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			while (source.Read())
			{
				Dictionary<string, object> row = new Dictionary<string, object>();
				for (int i = 0; i < source.FieldCount; i++)
				{
					object value = source.GetValue(i);
					// return an empty string for dbnull value of field type string
					if (source.GetFieldType(i) == typeof(string) && source.IsDBNull(i))
						value = string.Empty;
					row.Add(source.GetName(i), value);
				}
				yield return row;
			}
		}
	}
}


// suppose you have a class named MYBooks with properties names ID (int) and TITLE (string)
// and the datareader contains more records with two fileds named ID_BOOK and TITLE

//myDataReader.AsEnumerable().Select(i => new MYBooks()
//{
//	ID = (int)i["ID_BOOK"],
//	TITLE = (string)i["TITLE"]
//});