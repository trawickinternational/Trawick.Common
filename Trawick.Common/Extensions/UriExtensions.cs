using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Trawick.Common.Extensions
{
	public static class UriExtensions
	{
		public static RouteData ToRouteData(this Uri uri)
		{
			string path = new UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath).ToString();
			RouteData data = RouteTable.Routes.GetRouteData(
				new HttpContextWrapper(
					new HttpContext(
						new HttpRequest(null, path, uri.Query),
						new HttpResponse(new System.IO.StringWriter()))));
			return data;
		}

		public static RouteData ToRouteData(this string url)
		{
			if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
			{
				return new Uri(url).ToRouteData();
			}
			return null;
		}

		//public static RouteValueDictionary ToRouteValues(this string url)
		//{
		//	if (url == null)
		//	{
		//		if (!this.HttpContext.Request.Headers.ContainsKey("Referer"))
		//			throw new Exception("url parameter not delivered and Referer header is empty");
		//		url = this.HttpContext.Request.Headers["Referer"].ToString();
		//	}
		//	var route = this.RouteData.Routers.OfType<Route>().FirstOrDefault(p => p.Name == Startup.DefaultRouteName);
		//	var template = route.ParsedTemplate;
		//	var matcher = new TemplateMatcher(template, route.Defaults);
		//	var routeValues = new RouteValueDictionary();
		//	var localPath = (new Uri(url)).LocalPath;
		//	if (!matcher.TryMatch(localPath, routeValues))
		//		throw new Exception("Could not identity controller and action");
		//	return routeValues;
		//}



		//public RouteValueDictionary GetRouteValuesFromUrlOrReferer(string url = null)
		//{
		//	if (url == null)
		//	{
		//		if (!this.HttpContext.Request.Headers.ContainsKey("Referer"))
		//			throw new Exception("url parameter not delivered and Referer header is empty");
		//		url = this.HttpContext.Request.Headers["Referer"].ToString();
		//	}
		//	var route = this.RouteData.Routers.OfType<Route>().FirstOrDefault(p => p.Name == Startup.DefaultRouteName);
		//	var template = route.ParsedTemplate;
		//	var matcher = new TemplateMatcher(template, route.Defaults);
		//	var routeValues = new RouteValueDictionary();
		//	var localPath = (new Uri(url)).LocalPath;
		//	if (!matcher.TryMatch(localPath, routeValues))
		//		throw new Exception("Could not identity controller and action");
		//	return routeValues;
		//}


	}

}
