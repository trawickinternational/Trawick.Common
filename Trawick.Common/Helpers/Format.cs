using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Helpers
{
	public static class Format
	{

		public static string ToCurrency(string data)
		{
			return string.Format("{0:c}", decimal.Parse(data));
		}

		public static string ToCurrency(int? data)
		{
			return string.Format("{0:c}", data);
		}

		public static string ToCurrency(decimal? data)
		{
			return string.Format("{0:c}", data);
		}



		public static string ToShortDate(string data)
		{
			return Convert.ToDateTime(data).ToShortDateString();
		}

		public static string ToShortDate(DateTime? data)
		{
			return Convert.ToDateTime(data).ToShortDateString();
		}


	}
}
