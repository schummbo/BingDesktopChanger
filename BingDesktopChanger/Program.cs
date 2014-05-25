using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
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


                string urlBase;
                string copyright = string.Empty;

                try
                {
                    var image = xml.Element("image");

                    var urlBaseElement = image.Element("urlBase");

                    urlBase = urlBaseElement.Value;

                    var copyrightElement = image.Element("copyright");

                    if (copyrightElement != null)
                    {
                        copyright = copyrightElement.Value;
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("An exception occurred parsing xml from Bing. The format may have changed. Exception" + ex.Message);
                }


                try
                {
                    Wallpaper.Set(string.Format("{0}{1}_{2}", "http://www.bing.com", urlBase, "1920x1200.jpg"), (Wallpaper.Style)Enum.Parse(typeof(Wallpaper.Style),ConfigurationManager.AppSettings["WallpaperFitMode"]), copyright);
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred setting the wallpaper. Exception: " + ex.Message);
                }
            }
        }
    }
}
