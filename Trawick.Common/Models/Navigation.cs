using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Trawick.Common.Models
{
	public class Navigation
	{

		public List<NavLink> Children { get; set; }
		public bool Remote { get; set; }


		public Navigation(string url = "")
		{
			//ServicePointManager.Expect100Continue = false;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			this.Children = new List<NavLink>();
			if (!string.IsNullOrEmpty(url))
			{
				this.LoadSiteMap(url);
			}
		}


		public void LoadSiteMap(string url)
		{
			string rawXml = "";
			Uri target = new Uri(url);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(target);

			if ((request.GetResponse().ContentLength > 0))
			{
				var stream = new StreamReader(request.GetResponse().GetResponseStream());
				rawXml = stream.ReadToEnd();
				if (stream != null)
				{
					stream.Close();
				}
			}
			this.ParseSiteMap(rawXml);
		}


		public void ParseSiteMap(string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			// You have to use xml namespace specifically to select nodes
			var mgr = new XmlNamespaceManager(doc.NameTable);
			mgr.AddNamespace("ns", "http://www.sitemaps.org/schemas/sitemap/0.9");

			XmlNode root = doc.DocumentElement;
			XmlNodeList nodes = root.SelectNodes("//ns:url", mgr);

			foreach (XmlNode xn in nodes)
			{
				//string text = xn["title"].InnerText;
				var title = xn.Attributes["title"];
                if (title != null)
                {
                    string text = title.InnerText;
                    string href = xn["loc"].InnerText;

                    // returns { "/", "about-us/", "our-mission" }
                    string[] parts = new Uri(href).Segments;

                    int level = parts.Length - 2;
                    string parent = parts[level].TrimEnd('/');
                    string page = parts[level + 1].TrimEnd('/');

                    var link = new NavLink
                    {
                        Url = href,
                        Name = text,
                        Page = page,
                        Parent = parent
                    };

                    if (parent != "products")
                    {
                        this.Add(link);
                    }
                }
			}
		}


		public void Add(NavLink link)
		{
			var parent = Children.FirstOrDefault(x => x.Page == link.Parent);
			if (parent != null)
			{
				if (parent.Children == null)
				{
					parent.Children = new List<NavLink>();
				}
				link.Order = parent.Children.Count + 1;
				parent.Children.Add(link);
			}
			else
			{
				link.Order = this.Children.Count + 1;
				this.Children.Add(link);
			}
		}

	}// END Navigation



	public class NavLink
	{
		public string Name { get; set; }
		public string Url { get; set; }
		public string Page { get; set; }
		public string Parent { get; set; }
		public int Order { get; set; }

		public List<NavLink> Children { get; set; }
	}// END NavLink
}
