using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trawick.Common.Helpers
{
	public class StringHelper
	{
		public static string FSQ(string v)
		{
			if (v != null)
				return v.Replace("'", "''");
			else
				return "";
		}

		public static string RandomPassword
		{
			get
			{
				// generate random password from GUID
				string password = string.Empty;
				string guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
				Random random = new Random(DateTime.Now.Millisecond);
				for (int i = 0; i < 8; i++)
					password += guid.Substring(random.Next(0, guid.Length - 1), 1);
				return password;
			}
		}


	}
}
