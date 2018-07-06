using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Trawick.Common.Extensions
{
	public static class SelectListExtensions
	{

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