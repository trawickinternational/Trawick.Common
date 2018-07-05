using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Trawick.Common.Helpers
{
	public class SubdomainRoute : RouteBase
	{
		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
			if (httpContext.Request == null || httpContext.Request.Url == null)
			{
				return null;
			}

			// Grab the host from the headers
			var host = httpContext.Request.Url.Host;
			// Make sure there is a dot, otherwise return null
			var index = host.IndexOf(".");
			string[] segments = httpContext.Request.Url.PathAndQuery.TrimStart('/').Split('/');

			if (index < 0)
			{
				return null;
			}

			// Detect the subdomain (everything up until the first dot)
			// So we can have as many top domains as we need.
			var subdomain = host.Substring(0, index);
			string[] blacklist = { "www", "alum", "mail" };

			if (blacklist.Contains(subdomain))
			{
				return null;
			}

			string controller = (segments.Length > 0 && !string.IsNullOrEmpty(segments[0])) ? segments[0] : "Home";
			string action = (segments.Length > 1) ? segments[1] : "Index";

			var routeData = new RouteData(this, new MvcRouteHandler());
			routeData.Values.Add("controller", controller); //Goes to the relevant Controller  class
			routeData.Values.Add("action", action); //Goes to the relevant action method on the specified Controller
			routeData.Values.Add("subdomain", subdomain); //pass subdomain as argument to action method

			if (controller != "Home")
			{
				return null;
			}

			return routeData;
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			//Implement your formating Url formating here
			return null;
		}
	}







	// http://www.matrichard.com/post/asp.net-mvc-5-routing-with-subdomain

	//public class SubdomainRoute : Route
	//{
	//	public SubdomainRoute(string domain, string url, RouteValueDictionary defaults)
	//			: this(domain, url, defaults, new MvcRouteHandler())
	//	{
	//	}

	//	public SubdomainRoute(string domain, string url, object defaults)
	//			: this(domain, url, new RouteValueDictionary(defaults), new MvcRouteHandler())
	//	{
	//	}

	//	public SubdomainRoute(string domain, string url, object defaults, IRouteHandler routeHandler)
	//			: this(domain, url, new RouteValueDictionary(defaults), routeHandler)
	//	{
	//	}

	//	public SubdomainRoute(string domain, string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
	//			: base(url, defaults, routeHandler)
	//	{
	//		this.Domain = domain;
	//	}

	//	public string Domain { get; set; }

	//	public override RouteData GetRouteData(HttpContextBase httpContext)
	//	{
	//		var routeData = base.GetRouteData(httpContext);
	//		routeData.Values.Add("client", httpContext.Request.Url.Host.Split('.').First());
	//		return routeData;
	//	}
	//}
	//routes.Add("Subdomain", new SubdomainRoute(
	//               "{client}.test.local", //of course this should represent the real intent…like I said throwaway demo project in local IIS
	//               "{controller}/{action}/{id}",

	//							new { controller = "Home", action = "Index", id = UrlParameter.Optional
	//}));

}