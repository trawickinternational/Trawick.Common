using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Trawick.Common.Extensions
{
	public static class DateTimeExtensions
	{

		public static int AgeOn(this DateTime birthday, DateTime reference)
		{
			// Calculate the age.
			int age = reference.Year - birthday.Year;
			// Go back to the year the person was born in case of a leap year
			if (birthday > reference.AddYears(-age)) age--;
			//if (reference < birthday.AddYears(age)) age--;
			return age;
		}

		public static int AgeToday(this DateTime birthday)
		{
			return AgeOn(birthday, DateTime.Now);
		}


		public static DateTime StartOfWeek(this DateTime dateTime)
		{
			int dayOfWeek = (int)dateTime.DayOfWeek;
			dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;
			DateTime startOfWeek = dateTime.AddDays(1 - dayOfWeek);
			return startOfWeek;
		}

		// Returns the number of milliseconds since Jan 1, 1970 (useful for converting C# dates to JS dates)
		public static double ToUnixTimeStamp(this DateTime dateTime)
		{
			return dateTime
				 .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
				 .TotalMilliseconds;
		}




		public static string ToDateString(this object o, string defaultValue = "N/A")
		{
			DateTime result;
			if (DateTime.TryParse((o ?? "").ToString(), out result))
			{
				return result.ToShortDateString();
			}
			else
			{
				return defaultValue;
			}
		}

		public static DateTime ToDateTime(this object o, DateTime defaultValue)
		{
			DateTime result;
			if (DateTime.TryParse((o ?? "").ToString(), out result))
			{
				return result;
			}
			else
			{
				return defaultValue;
			}
		}







		//public static DateTime ToDateTime(this string dateString, string format = "MM/dd/yyyy", string cultureString = "en-US")
		//{
		//	try
		//	{
		//		var culture = CultureInfo.GetCultureInfo(cultureString);
		//		return DateTime.ParseExact(s: dateString, format: format, provider: culture);
		//	}
		//	catch (FormatException)
		//	{
		//		throw;
		//	}
		//	catch (CultureNotFoundException)
		//	{
		//		throw; // Given Culture is not supported culture
		//	}
		//}

		//public static DateTime ToDateTime(this string dateString, string format, CultureInfo culture)
		//{
		//	try
		//	{
		//		return DateTime.ParseExact(s: dateString, format: format, provider: culture);
		//	}
		//	catch (FormatException)
		//	{
		//		throw;
		//	}
		//	catch (CultureNotFoundException)
		//	{
		//		throw; // Given Culture is not supported culture
		//	}
		//}

	}
}