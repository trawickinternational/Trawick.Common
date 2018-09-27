using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Trawick.Common.Extensions
{
	// ASP.NET [C#] REDIRECT WITH POST DATA
	public static class WebExtensions
	{
		public static void RedirectPost(string redirect, NameValueCollection data)
		{
			HttpResponse response = HttpContext.Current.Response;
			response.Clear();

			StringBuilder strForm = new StringBuilder();
			strForm.Append("<html><head></head>");
			strForm.AppendFormat("<body onload=\"document.form1.submit();\">");
			strForm.AppendFormat("<form name=\"form1\" action=\"{0}\" method=\"post\">", redirect);
			foreach (string key in data)
			{
				strForm.AppendFormat("<input type=\"hidden\" name=\"{0}\" value=\"{1}\">", key, data[key]);
			}
			strForm.Append("</form></body></html>");
			response.Write(strForm.ToString());
			response.End();
		}
	}
}
