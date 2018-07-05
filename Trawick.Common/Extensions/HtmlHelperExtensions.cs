using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Trawick.Common.Extensions
{
	public static class HtmlHelperExtensions
	{

		public static MvcHtmlString BsActionLink(this HtmlHelper html, string text, string action, string controller, object routeValues = null, object css = null)
		{
			var context = html.ViewContext;
			if (context.Controller.ControllerContext.IsChildAction)
				context = html.ViewContext.ParentActionViewContext;

			var routeDataValues = context.RouteData.Values;
			var currentAction = routeDataValues["action"].ToString().ToLower();
			var currentController = routeDataValues["controller"].ToString().ToLower();

			string url = (routeValues == null) ?
				(new UrlHelper(html.ViewContext.RequestContext)).Action(action, controller) :
				(new UrlHelper(html.ViewContext.RequestContext)).Action(action, controller, routeValues);

			var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(css);
			TagBuilder aTag = new TagBuilder("a");

			if (currentAction.Equals(action.ToLower(), StringComparison.InvariantCulture) &&
				currentController.Equals(controller.ToLower(), StringComparison.InvariantCulture))
			{
				htmlAttributes["class"] += " active";
			}

			aTag.MergeAttribute("href", url);
			aTag.InnerHtml = text;
			aTag.MergeAttributes(htmlAttributes);

			return new MvcHtmlString(aTag.ToString(TagRenderMode.Normal));
		}


		public static IDictionary<string, object> MergeHtmlAttributes(this HtmlHelper helper, object htmlAttributesObject, object defaultHtmlAttributesObject)
		{
			var concatKeys = new string[] { "class" };
			var htmlAttributesDict = htmlAttributesObject as IDictionary<string, object>;
			var defaultHtmlAttributesDict = defaultHtmlAttributesObject as IDictionary<string, object>;

			RouteValueDictionary htmlAttributes = (htmlAttributesDict != null)
					? new RouteValueDictionary(htmlAttributesDict)
					: HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributesObject);
			RouteValueDictionary defaultHtmlAttributes = (defaultHtmlAttributesDict != null)
					? new RouteValueDictionary(defaultHtmlAttributesDict)
					: HtmlHelper.AnonymousObjectToHtmlAttributes(defaultHtmlAttributesObject);

			foreach (var item in htmlAttributes)
			{
				if (concatKeys.Contains(item.Key))
				{
					defaultHtmlAttributes[item.Key] = (defaultHtmlAttributes[item.Key] != null)
							? string.Format("{0} {1}", defaultHtmlAttributes[item.Key], item.Value)
							: item.Value;
				}
				else
				{
					defaultHtmlAttributes[item.Key] = item.Value;
				}
			}
			return defaultHtmlAttributes;
		}
		// https://cpratt.co/html-editorfor-and-htmlattributes/

	}
}





//https://stackoverflow.com/questions/703281/getting-path-relative-to-the-current-working-directory

//public static string GetRelativePathFrom(this FileSystemInfo to, FileSystemInfo from)
//{
//	return from.GetRelativePathTo(to);
//}

//public static string GetRelativePathTo(this FileSystemInfo from, FileSystemInfo to)
//{
//	Func<FileSystemInfo, string> getPath = fsi =>
//	{
//		var d = fsi as DirectoryInfo;
//		return d == null ? fsi.FullName : d.FullName.TrimEnd('\\') + "\\";
//	};

//	var fromPath = getPath(from);
//	var toPath = getPath(to);

//	var fromUri = new Uri(fromPath);
//	var toUri = new Uri(toPath);

//	var relativeUri = fromUri.MakeRelativeUri(toUri);
//	var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

//	return relativePath.Replace('/', Path.DirectorySeparatorChar);
//}

