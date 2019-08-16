using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Trawick.Common.Extensions
{
	public static class HtmlHelperExtensions
	{

		public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string partialViewName)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			object model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;
			var viewData = new ViewDataDictionary(helper.ViewData)
			{
				TemplateInfo = new TemplateInfo
				{
					HtmlFieldPrefix = name
				}
			};
			//if (model == null)
			//{
			//	return null;
			//}
			return helper.Partial(partialViewName, model, viewData);
		}
		// @Html.PartialFor(model => model.Child, "_AnotherViewModelControl")





		// ActionLink for Bootstrap. Adds "active" class if current page and converts all text to HtmlString, so it can include markup.
		public static MvcHtmlString BsActionLink(this HtmlHelper html, string text, string action, string controller, object routeValues = null, object htmlAttr = null)
		{
			var context = html.ViewContext;
			if (context.Controller.ControllerContext.IsChildAction)
				context = html.ViewContext.ParentActionViewContext;

			var routeDataValues = context.RouteData.Values;

			string currentArea = string.Empty;
			if (context.RouteData.DataTokens.ContainsKey("area"))
			{
				currentArea = context.RouteData.DataTokens["area"].ToString().ToLower();
			}
			var currentAction = routeDataValues["action"].ToString().ToLower();
			var currentController = routeDataValues["controller"].ToString().ToLower();

			string url = (routeValues == null) ?
				(new UrlHelper(html.ViewContext.RequestContext)).Action(action, controller) :
				(new UrlHelper(html.ViewContext.RequestContext)).Action(action, controller, routeValues);

			var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttr);
			TagBuilder aTag = new TagBuilder("a");

			var area = GetDynamicProperty(routeValues, "area");

			bool isArea = currentArea.Equals(area.ToLower(), StringComparison.InvariantCulture);
			bool isController = currentController.Equals(controller.ToLower(), StringComparison.InvariantCulture);
			bool isAction = currentAction.Equals(action.ToLower(), StringComparison.InvariantCulture);

			if (isArea && isController && isAction)
			{
				htmlAttributes["class"] += " active";
			}

			aTag.MergeAttribute("href", url);
			aTag.InnerHtml = text;
			aTag.MergeAttributes(htmlAttributes);

			return new MvcHtmlString(aTag.ToString(TagRenderMode.Normal));
		}


		public static string GetDynamicProperty(dynamic args, string name)
		{
			if (args == null) return string.Empty;
			var propertyInfo = args.GetType().GetProperty(name);
			var value = propertyInfo.GetValue(args, null);
			return value != null ? value.ToString() : string.Empty;
		}

		public static bool PropertyExists(dynamic args, string name)
		{
			if (args == null) return false;
			if (args is IDictionary<string, object>)
			{
				return (args as IDictionary<string, object>).ContainsKey(name);
				//return dict.ContainsKey(name);
			}
			return args.GetType().GetProperty(name) != null;
		}




		// Good for use inside page views.
		/// <summary>
		/// Gets an object containing a htmlAttributes collection for any Razor HTML helper component,
		/// supporting a static set (anonymous object) and/or a dynamic set (Dictionary)
		/// </summary>
		/// <param name="fixedHtmlAttributes">A fixed set of htmlAttributes (anonymous object)</param>
		/// <param name="dynamicHtmlAttributes">A dynamic set of htmlAttributes (Dictionary)</param>
		/// <returns>A collection of htmlAttributes including a merge of the given set(s)</returns>
		public static IDictionary<string, object> ExtendHtmlAttributes(this HtmlHelper helper, object fixedHtmlAttrs = null, IDictionary<string, object> dynamicHtmlAttrs = null)
		{
			var rvd = (fixedHtmlAttrs == null)
					? new RouteValueDictionary()
					: HtmlHelper.AnonymousObjectToHtmlAttributes(fixedHtmlAttrs);
			if (dynamicHtmlAttrs != null)
			{
				foreach (KeyValuePair<string, object> kvp in dynamicHtmlAttrs)
				{
					rvd[kvp.Key] = kvp.Value;
				}
			}
			return rvd;
		}
		//var dic = new Dictionary<string, object>();
		//if (IsReadOnly()) dic.Add("readonly", "readonly");
		//Html.TextBoxFor(m => m.Name, ExtendHtmlAttributes(new { @class="someclass" }, dic))



		// Good for use inside EditorTemplates.
		public static IDictionary<string, object> MergeHtmlAttributes(this HtmlHelper helper, object htmlAttr, object defaultHtmlAttr)
		{
			var concatKeys = new string[] { "class" };
			var htmlAttributesDict = htmlAttr as IDictionary<string, object>;
			var defaultHtmlAttributesDict = defaultHtmlAttr as IDictionary<string, object>;

			RouteValueDictionary htmlAttributes = (htmlAttributesDict != null)
					? new RouteValueDictionary(htmlAttributesDict)
					: HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttr);
			RouteValueDictionary defaultHtmlAttributes = (defaultHtmlAttributesDict != null)
					? new RouteValueDictionary(defaultHtmlAttributesDict)
					: HtmlHelper.AnonymousObjectToHtmlAttributes(defaultHtmlAttr);

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


		public static MvcHtmlString BootstrapCheckBoxFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, bool>> expression)
		{
			TagBuilder innerContainer = new TagBuilder("div");
			innerContainer.AddCssClass("col-sm-5");
			innerContainer.InnerHtml = helper.CheckBoxFor(expression, new { @class = "form-control" }).ToString();

			StringBuilder html = new StringBuilder();
			html.Append(helper.LabelFor(expression, new { @class = "col-sm-3 control-label" }));
			html.Append(innerContainer.ToString());

			TagBuilder outerContainer = new TagBuilder("div");
			outerContainer.AddCssClass("form-group");
			outerContainer.InnerHtml = html.ToString();

			return MvcHtmlString.Create(outerContainer.ToString());
		}
		// https://stackoverflow.com/questions/27303529/creating-a-helper-to-override-checkboxfor


		// Just Expanding to Include Nullable Booleans
		public static MvcHtmlString CheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool?>> expression)
		{
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			bool? value = (modelMeta.Model as bool?);
			string name = ExpressionHelper.GetExpressionText(expression);
			return htmlHelper.CheckBox(name, value ?? false);
		}

		public static MvcHtmlString CheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool?>> expression, object htmlAttributes)
		{
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			bool? value = (modelMeta.Model as bool?);
			string name = ExpressionHelper.GetExpressionText(expression);
			return htmlHelper.CheckBox(name, value ?? false, htmlAttributes);
		}

		public static MvcHtmlString CheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool?>> expression, IDictionary<string, object> htmlAttributes)
		{
			ModelMetadata modelMeta = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			bool? value = (modelMeta.Model as bool?);
			string name = ExpressionHelper.GetExpressionText(expression);
			return htmlHelper.CheckBox(name, value ?? false, htmlAttributes);
		}


		public static MvcHtmlString RadioButtonForSelectList<TModel, TProperty>(
		this HtmlHelper<TModel> htmlHelper,
		Expression<Func<TModel, TProperty>> expression,
		IEnumerable<SelectListItem> listOfValues,
		string rbClassName = "Horizontal")
		{
			var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			var sb = new StringBuilder();

			if (listOfValues != null)
			{
				// Create a radio button for each item in the list 
				foreach (SelectListItem item in listOfValues)
				{
					// Generate an id to be given to the radio button field 
					var id = string.Format("{0}_{1}", metaData.PropertyName, item.Value);

					// Create and populate a radio button using the existing html helpers 
					var label = htmlHelper.Label(id, HttpUtility.HtmlEncode(item.Text));
					var radio = string.Empty;

					if (item.Selected == true)
					{
						radio = htmlHelper.RadioButtonFor(expression, item.Value, new { id = id, @checked = "checked" }).ToHtmlString();
					}
					else
					{
						radio = htmlHelper.RadioButtonFor(expression, item.Value, new { id = id }).ToHtmlString();
					}
					// Create the html string to return to client browser
					// e.g. <input data-val="true" data-val-required="You must select an option" id="RB_1" name="RB" type="radio" value="1" /><label for="RB_1">Choice 1</label> 
					sb.AppendFormat("<div class=\"RB{2}\">{0}{1}</div>", radio, label, rbClassName);
				}
			}
			return MvcHtmlString.Create(sb.ToString());
		}
		// @Html.RadioButtonForSelectList(m => m.Sexsli, (SelectList)Model.Sexsli, "Sex") 




		//public static MvcHtmlString KeywordDropDownListFor<TModel, TProperty>(
		//	this HtmlHelper<TModel> htmlHelper, 
		//	Expression<Func<TModel, TProperty>> expression, 
		//	IEnumerable<SelectListItem> selectList, 
		//	object htmlAttributes)
		//{
		//	Func<TModel, TProperty> method = expression.Compile();
		//	string value = method(htmlHelper.ViewData.Model) as string;

		//	if (string.IsNullOrEmpty(value))
		//	{
		//		List<SelectListItem> newItems = new List<SelectListItem>();
		//		newItems.Add(new SelectListItem
		//		{
		//			Selected = true,
		//			Text = Strings.ChooseAKeyword,
		//			Value = string.Empty
		//		});
		//		foreach (SelectListItem item in selectList)
		//		{
		//			newItems.Add(item);
		//		}

		//		return htmlHelper.DropDownListFor(expression, newItems, htmlAttributes);
		//	}

		//	return htmlHelper.DropDownListFor(expression, selectList, htmlAttributes);
		//}

	}
}


// https://johnnycode.com/2014/10/03/how-to-get-the-current-controller-name-action-or-id-asp-net-mvc/


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

