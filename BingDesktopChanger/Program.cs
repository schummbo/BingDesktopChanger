using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace BingDesktopChanger
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var webClient = new WebClient { Encoding = Encoding.UTF8 })
			{
				XElement xml;

				try
				{
					var xmlString = webClient.DownloadString("http://www.bing.com/hpimagearchive.aspx?format=xml&idx=0&n=1&mbl=1&mkt=en-ww");
					xml = XElement.Parse(xmlString);
				}
				catch (Exception ex)
				{
					throw new Exception("An exception occurred getting xml from Bing. The url may have changed. Exception: " + ex.Message);
				}

				var image = xml.Element("image");

				if (image != null)
				{
					var startDateElement = image.Element("startdate");
					if (startDateElement != null)
					{
						var effectiveDate = DateTime.ParseExact(startDateElement.Value, "yyyyMMdd", CultureInfo.InvariantCulture);

						if (DateTime.Now >= effectiveDate)
						{
							var urlBaseElement = image.Element("urlBase");

							if (urlBaseElement != null)
							{

                                var copyright = image.Element("copyright").Value;

								try
								{
									Wallpaper.Set(string.Format("{0}{1}_{2}", "http://www.bing.com", urlBaseElement.Value, "1920x1200.jpg"), Wallpaper.Style.Stretched, copyright);
								}
								catch (Exception ex)
								{
									throw new Exception("An error occurred setting the wallpaper. Exception: " + ex.Message);
								}
							}
						}
					}
				}
			}
		}
	}
}
