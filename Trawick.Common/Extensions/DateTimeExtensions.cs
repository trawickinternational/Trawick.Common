using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trawick.Common.Extensions
{
	public static class DateTimeExtensions
	{

		public static int AgeOn(this DateTime birthday, DateTime reference)
		{
			int age = reference.Year - birthday.Year;
			if (reference < birthday.AddYears(age)) age--;

			return age;
		}


		public static DateTime StartOfWeek(this DateTime dateTime)
		{
			int dayOfWeek = (int)dateTime.DayOfWeek;
			dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;
			DateTime startOfWeek = dateTime.AddDays(1 - dayOfWeek);

			return startOfWeek;
		}

		// returns the number of milliseconds since Jan 1, 1970 (useful for converting C# dates to JS dates)
		public static double ToUnixTimeStamp(this DateTime dateTime)
		{
			return dateTime
				 .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
				 .TotalMilliseconds;
		}
	}
}