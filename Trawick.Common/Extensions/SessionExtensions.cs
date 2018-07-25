using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace Trawick.Common.Extensions
{
	public static class SessionExtensions
	{
		/// <summary>
		/// this class saves something to the Session object
		/// but with an EXPIRATION TIMEOUT
		/// (just like the ASP.NET Cache)
		/// usage sample:
		///  Session.AddWithTimeout(
		///   "key",
		///   "value",
		///   TimeSpan.FromMinutes(5));
		/// </summary>
		public static void AddWithTimeout(this HttpSessionState session, string name, object value, TimeSpan expireAfter)
		{
			lock (session)
			{
				session[name] = value;
			}

			//add cleanup task that will run after "expire"
			Task.Delay(expireAfter).ContinueWith((task) => {
				lock (session)
				{
					session.Remove(name);
				}
			});
		}


		public static void AddWithTimeout(this HttpSessionStateBase session, string name, object value, TimeSpan expireAfter)
		{
			lock (session)
			{
				session[name] = value;
			}

			//add cleanup task that will run after "expire"
			Task.Delay(expireAfter).ContinueWith((task) => {
				lock (session)
				{
					session.Remove(name);
				}
			});
		}
		//store and expire after 5 minutes
		//Session.AddWithTimeout("key", "value", TimeSpan.FromMinutes(5));

		//get the stored value just normally
		//var x = Session["key"];


	}
}
