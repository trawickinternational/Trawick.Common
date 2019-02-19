using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Trawick.Common.Extensions
{
	public static class StringExtensions
	{

		#region Type Detection


		/// <summary>
		///		Checks if the String contains only Unicode letters.
		///		A null or empty String ("") will return false.
		/// </summary>
		/// <param name="value">string to check if is Alpha</param>
		/// <returns>true if only contains letters, and is non-null</returns>
		public static bool IsAlpha(this string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			return value.Trim().Replace(" ", "").All(char.IsLetter);
		}


		/// <summary>
		///		Checks if the String contains only Unicode letters, digits.
		///		A null or empty String ("") will return false.
		/// </summary>
		/// <param name="value">string to check if is Alpha or Numeric</param>
		/// <returns></returns>
		public static bool IsAlphaNumeric(this string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			return value.Trim().Replace(" ", "").All(char.IsLetterOrDigit);
		}


		/// <summary>
		///		Checks if the String is a valid email address.
		/// </summary>
		/// <param name="value">string email address</param>
		/// <returns>true or false if email if valid</returns>
		public static bool IsEmailAddress(this string value)
		{
			string pattern = "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$";
			return Regex.Match(value, pattern).Success;
		}


		/// <summary>
		///		Checks if date with dateFormat is parse-able to System.DateTime format returns boolean value if true else false.
		/// </summary>
		/// <param name="value">String date</param>
		/// <param name="dateFormat">date format example dd/MM/yyyy HH:mm:ss</param>
		/// <returns>boolean True False if is valid System.DateTime</returns>
		public static bool IsDateTime(this string value, string dateFormat)
		{
			DateTime output = default(DateTime);
			return DateTime.TryParseExact(value, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out output);
		}


		/// <summary>
		///		IsInteger Function checks if a string is a valid int32 value.
		/// </summary>
		/// <param name="value">val</param>
		/// <returns>Boolean True if isInteger else False</returns>
		public static bool IsInteger(this string value)
		{
			int output;
			return Int32.TryParse(value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out output);
		}


		/// <summary>
		///		Checks if the String is a valid floating value.
		/// </summary>
		/// <param name="value"></param>
		/// <returns>Boolean True if isNumeric else False</returns>
		/// <remarks></remarks>
		public static bool IsNumeric(this string value)
		{
			double output;
			return Double.TryParse(value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out output);
        }


        #endregion


        #region Encrypt/Decrypt


        /// <summary>
        ///		Encrypt a string using the supplied key. Encoding is done using RSA encryption.
        /// </summary>
        /// <param name="value">String that must be encrypted.</param>
        /// <param name="key">Encryption key</param>
        /// <returns>A string representing a byte array separated by a minus sign.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToEncrypt or key is null or empty.</exception>
        public static string Encrypt(this string value, string key)
		{
			var cspParameter = new CspParameters { KeyContainerName = key };
			var rsaServiceProvider = new RSACryptoServiceProvider(cspParameter) { PersistKeyInCsp = true };
			byte[] bytes = rsaServiceProvider.Encrypt(Encoding.UTF8.GetBytes(value), true);
			return BitConverter.ToString(bytes);
		}


		/// <summary>
		///		Decrypt a string using the supplied key. Decoding is done using RSA encryption.
		/// </summary>
		/// <param name="value">String that must be decrypted.</param>
		/// <param name="key">Decryption key.</param>
		/// <returns>The decrypted string or null if decryption failed.</returns>
		/// <exception cref="ArgumentException">Occurs when stringToDecrypt or key is null or empty.</exception>
		public static string Decrypt(this string value, string key)
		{
			var cspParamters = new CspParameters { KeyContainerName = key };
			var rsaServiceProvider = new RSACryptoServiceProvider(cspParamters) { PersistKeyInCsp = true };
			string[] decryptArray = value.Split(new[] { "-" }, StringSplitOptions.None);
			byte[] decryptByteArray = Array.ConvertAll(decryptArray, (s => Convert.ToByte(byte.Parse(s, NumberStyles.HexNumber))));
			byte[] bytes = rsaServiceProvider.Decrypt(decryptByteArray, true);
			string result = Encoding.UTF8.GetString(bytes);
			return result;
		}


		#endregion


		#region Json/QueryString


		/// <summary>
		///		Converts a Json string to dictionary object method applicable for single hierarchy objects i.e
		///		no parent child relationships.
		/// </summary>
		/// <param name="value">string formated as Json</param>
		/// <returns>IDictionary Json object</returns>
		/// <remarks>
		///		<exception cref="ArgumentNullException">if string parameter is null or empty</exception>
		/// </remarks>
		public static IDictionary<string, object> JsonToDictionary(this string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}
			return (Dictionary<string, object>)JsonConvert.DeserializeObject(value, typeof(Dictionary<string, object>));
		}


		/// <summary>
		///		Converts a Json string to object of type T method applicable for multi hierarchy objects i.e
		///		having zero or many parent child relationships, Ignore loop references and do not serialize if cycles are detected.
		/// </summary>
		/// <typeparam name="T">object to convert to</typeparam>
		/// <param name="value">json</param>
		/// <returns>object</returns>
		public static T JsonToObject<T>(this string value)
		{
			var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
			return JsonConvert.DeserializeObject<T>(value, settings);
		}


		/// <summary>
		///		Convert url query string to IDictionary value key pair.
		/// </summary>
		/// <param name="value">query string value</param>
		/// <returns>IDictionary value key pair</returns>
		public static IDictionary<string, string> QueryStringToDictionary(this string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return null;
			}
			if (!value.Contains("?"))
			{
				return null;
			}
			string query = value.Replace("?", "");
			if (!query.Contains("="))
			{
				return null;
			}
			return query.Split('&').Select(p => p.Split('=')).ToDictionary(key => key[0].ToLower().Trim(), val => val[1]);
		}

        #endregion





        public static bool ContainsAny(this string value, IEnumerable<string> searchWords)
        {
            return searchWords.Any(value.Contains);
        }

        public static bool ContainsAny(this string value, string[] searchWords)
        {
            return searchWords.Any(value.Contains);
        }


        public static bool ContainsAll(this string value, IEnumerable<string> searchWords)
        {
            return searchWords.All(value.Contains);
        }

        public static bool ContainsAll(this string value, string[] searchWords)
        {
            return searchWords.All(value.Contains);
        }




        public static bool IsUpper(this string value)
        {
            //Consider string to be uppercase if it has no lowercase letters.
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsLower(value[i]))
                {
                    return false;
                }
            }
            return true;
            //return value.All(char.IsLower);
        }


        public static bool IsLower(this string value)
        {
            // Consider string to be lowercase if it has no uppercase letters.
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsUpper(value[i]))
                {
                    return false;
                }
            }
            return true;
            //return value.All(char.IsUpper);
        }



        public static string ToTitleCase(this string value)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
		}


		/// <summary>
		///		Read in a sequence of words from standard input and capitalize each
		///		one (make first letter uppercase; make rest lowercase).
		/// </summary>
		/// <param name="value">string</param>
		/// <returns>Word with capitalization</returns>
		public static string Capitalize(this string value)
		{
			if (value.Length == 0)
			{
				return value;
			}
			return value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower();
		}


		/// <summary>
		///		Truncate String and append ... at end
		/// </summary>
		/// <param name="value">String to be truncated</param>
		/// <param name="maxLength">number of chars to truncate</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string Truncate(this string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value) || maxLength <= 0)
			{
				return string.Empty;
			}
			if (value.Length > maxLength)
			{
				return value.Substring(0, maxLength) + "...";
			}
			return value;
		}


		/// <summary>
		///		Checks if a string is null or empty
		/// </summary>
		/// <param name="value">string to evaluate</param>
		/// <returns>true if string is null or is empty else false</returns>
		public static bool IsNullOrEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}


		/// <summary>
		///		Function returns a default String value if given value is null or empty
		/// </summary>
		/// <param name="value">String value to check if isEmpty</param>
		/// <param name="defaultValue">default value to return if String value isEmpty</param>
		/// <returns>returns either String value or default value if IsEmpty</returns>
		/// <remarks></remarks>
		public static string GetValueOrDefault(this string value, string defaultValue = null)
		{
			if (!string.IsNullOrEmpty(value))
			{
				value = value.Trim();
				return value.Length > 0 ? value : defaultValue;
			}
			return defaultValue;
		}


		/// <summary>
		///		Appends String quotes for type CSV data
		/// </summary>
		/// <param name="value">value</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string ParseStringToCsv(this string value)
		{
			return '"' + value.GetValueOrDefault("").Replace("\"", "\"\"") + '"';
		}


		/// <summary>
		///		ToTextElements
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static IEnumerable<string> ToTextElements(this string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			TextElementEnumerator elementEnumerator = StringInfo.GetTextElementEnumerator(value);
			while (elementEnumerator.MoveNext())
			{
				string textElement = elementEnumerator.GetTextElement();
				yield return textElement;
			}
		}


	}
}
