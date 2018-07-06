using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace Trawick.Common.Extensions
{
	public static class SectionExtensions
	{
		private static readonly object _o = new object();

		public static HelperResult RenderSection(this WebPageBase page, string sectionName, Func<object, HelperResult> defaultContent)
		{
			if (page.IsSectionDefined(sectionName))
			{
				return page.RenderSection(sectionName);
			}
			else {
				return defaultContent(_o);
			}
		}

		public static HelperResult RedefineSection(this WebPageBase page, string sectionName)
		{
			return RedefineSection(page, sectionName, defaultContent: null);
		}

		public static HelperResult RedefineSection(this WebPageBase page, string sectionName, Func<object, HelperResult> defaultContent)
		{
			if (page.IsSectionDefined(sectionName))
			{
				page.DefineSection(sectionName, () => page.Write(page.RenderSection(sectionName)));
			}
			else if (defaultContent != null)
			{
				page.DefineSection(sectionName, () => page.Write(defaultContent(_o)));
			}
			return new HelperResult(_ => { });
		}
	}
}

/*
Inside Area _Layout.cshtml


@using Trawick.Common.Extensions;

@{
	Layout = "~/Views/Shared/_Layout.cshtml";
}

<nav class="navbar navbar-expand-lg navbar-dark bg-dark">
	<div class="container">
		<ul class="navbar-nav nav-fade">
			<li class="nav-item">@Html.BsActionLink("<i class='fa fa-cog'></i>", "Index", "Services", new { area = "Member" }, new { @class = "nav-link", title = "Member Services" })</li>
			<li class="nav-item">@Html.BsActionLink("Account Status", "Status", "Services", new { area = "Member" }, new { @class = "nav-link" })</li>
			<li class="nav-item">@Html.BsActionLink("View/Print Forms", "Forms", "Services", new { area = "Member" }, new { @class = "nav-link" })</li>
			<li class="nav-item">@Html.BsActionLink("Get ID Card", "Card", "Services", new { area = "Member" }, new { @class = "nav-link" })</li>
			<li class="nav-item">@Html.BsActionLink("Update Member Info", "Update", "Services", new { area = "Member" }, new { @class = "nav-link" })</li>
			@*<li class="nav-item">@Html.BsActionLink("Claims", "Claims", "Services", new { area = "Member" }, new { @class = "nav-link" })</li>*@
		</ul>
	</div>
</nav>

@RenderBody()

@this.RedefineSection("styles")
@this.RedefineSection("scripts")
@this.RedefineSection("modals")

*/
