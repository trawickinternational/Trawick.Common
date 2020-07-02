using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Trawick.Common.Extensions
{
	public static class SelectListExtensions
	{

		public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> source, Func<T, string> valueFunc, Func<T, string> textFunc)
		{
			return source.Select(x => new SelectListItem
			{
				Value = valueFunc(x),
				Text = textFunc(x)
			});
		}
		//@Html.DropDownListFor(m => m.Item, Model.MyList.ToSelectList(x => x.ValueProperty, x => x.TextProperty))
		//@Html.DropDownListFor(m => m.Item, Model.MyList.ToSelectList(x => x.ValueProperty, x => x.TextProperty), "Choose...", new { @class = "form-control dropdown" })


		public static SelectList SetValue(this SelectList selectList, object selectedValue)
		{
			if (selectedValue != null)
			{
				return new SelectList(selectList.Items, selectList.DataValueField, selectList.DataTextField, selectedValue);
			}
			return selectList;
		}


		public static SelectList CurrencyOnly(this SelectList selectList)
		{
			var newList = new List<SelectListItem>();
			string newValue = "";
			foreach (var item in selectList)
			{
				try
				{
					newValue = item.Text.Split('$').Last().Split(' ').First().Replace(",", "");
					// this should try to return ONLY digits grouped together
				}
				catch
				{
					newValue = item.Text;
				}
				newList.Add(new SelectListItem
				{
					Text = string.Format("{0:C0}", Convert.ToDecimal(newValue)),
					Value = newValue,
					Selected = item.Selected
				});
			}
			return new SelectList(newList, "Value", "Text");
		}
	}
}





// https://stackoverflow.com/questions/7536631/adding-html-class-tag-under-option-in-html-dropdownlist/7537628#7537628

//public class ExtendedSelectListItem : SelectListItem
//{
//    public object HtmlAttributes { get; set; }
//}

//public static class ExtendedSelectExtensions
//{
//    internal static object GetModelStateValue(this HtmlHelper htmlHelper, string key, Type destinationType)
//    {
//        System.Web.Mvc.ModelState modelState;
//        if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
//        {
//            if (modelState.Value != null)
//            {
//                return modelState.Value.ConvertTo(destinationType, null /* culture */);
//            }
//        }
//        return null;
//    }

//    public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<ExtendedSelectListItem> selectList)
//    {
//        return DropDownList(htmlHelper, name, selectList, (string)null, (IDictionary<string, object>)null);
//    }

//    public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<ExtendedSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
//    {
//        return ExtendedDropDownListHelper(htmlHelper, null, name, selectList, optionLabel, htmlAttributes);
//    }

//    public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<ExtendedSelectListItem> selectList, string optionLabel, object htmlAttributes)
//    {
//        if (expression == null)
//            throw new ArgumentNullException("expression");
//        ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
//        return SelectInternal(htmlHelper, metadata, optionLabel, ExpressionHelper.GetExpressionText(expression), selectList,
//            false /* allowMultiple */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
//    }


//    public static MvcHtmlString ExtendedDropDownListHelper(this HtmlHelper htmlHelper, 
//        ModelMetadata metadata, 
//        string expression, 
//        IEnumerable<ExtendedSelectListItem> selectList, 
//        string optionLabel, 
//        IDictionary<string, object> htmlAttributes)
//    {
//        return SelectInternal(htmlHelper, metadata, optionLabel, expression, selectList, false, htmlAttributes);
//    }

//    private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, 
//        ModelMetadata metadata, 
//        string optionLabel, 
//        string name,
//        IEnumerable<ExtendedSelectListItem> selectList, 
//        bool allowMultiple,
//        IDictionary<string, object> htmlAttributes)
//    {
//        string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
//        if (String.IsNullOrEmpty(fullName))
//            throw new ArgumentException("No name");

//        if (selectList == null)
//            throw new ArgumentException("No selectlist");

//        object defaultValue = (allowMultiple)
//            ? htmlHelper.GetModelStateValue(fullName, typeof(string[]))
//            : htmlHelper.GetModelStateValue(fullName, typeof(string));

//        // If we haven't already used ViewData to get the entire list of items then we need to
//        // use the ViewData-supplied value before using the parameter-supplied value.
//        if (defaultValue == null)
//            defaultValue = htmlHelper.ViewData.Eval(fullName);

//        if (defaultValue != null)
//        {
//            IEnumerable defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
//            IEnumerable<string> values = from object value in defaultValues
//                                         select Convert.ToString(value, CultureInfo.CurrentCulture);
//            HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
//            List<ExtendedSelectListItem> newSelectList = new List<ExtendedSelectListItem>();

//            foreach (ExtendedSelectListItem item in selectList)
//            {
//                item.Selected = (item.Value != null)
//                    ? selectedValues.Contains(item.Value)
//                    : selectedValues.Contains(item.Text);
//                newSelectList.Add(item);
//            }
//            selectList = newSelectList;
//        }

//        // Convert each ListItem to an <option> tag
//        StringBuilder listItemBuilder = new StringBuilder();

//        // Make optionLabel the first item that gets rendered.
//        if (optionLabel != null)
//            listItemBuilder.Append(
//                ListItemToOption(new ExtendedSelectListItem()
//                {
//                    Text = optionLabel,
//                    Value = String.Empty,
//                    Selected = false
//                }));

//        foreach (ExtendedSelectListItem item in selectList)
//        {
//            listItemBuilder.Append(ListItemToOption(item));
//        }

//        TagBuilder tagBuilder = new TagBuilder("select")
//        {
//            InnerHtml = listItemBuilder.ToString()
//        };
//        tagBuilder.MergeAttributes(htmlAttributes);
//        tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
//        tagBuilder.GenerateId(fullName);
//        if (allowMultiple)
//            tagBuilder.MergeAttribute("multiple", "multiple");

//        // If there are any errors for a named field, we add the css attribute.
//        System.Web.Mvc.ModelState modelState;
//        if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
//        {
//            if (modelState.Errors.Count > 0)
//            {
//                tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
//            }
//        }

//        tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(fullName, metadata));

//        return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
//    }




//    internal static string ListItemToOption(ExtendedSelectListItem item)
//    {
//        TagBuilder builder = new TagBuilder("option")
//        {
//            InnerHtml = HttpUtility.HtmlEncode(item.Text)
//        };
//        if (item.Value != null)
//        {
//            builder.Attributes["value"] = item.Value;
//        }
//        if (item.Selected)
//        {
//            builder.Attributes["selected"] = "selected";
//        }
//        builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(item.HtmlAttributes));
//        return builder.ToString(TagRenderMode.Normal);
//    }

//}