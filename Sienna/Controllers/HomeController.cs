using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Sienna.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Design()
        {
            return View();
        }
        public ActionResult Exterior()
        {
            return PartialView();
        }
        public ActionResult Interior()
        {
            return PartialView();
        }
        public ActionResult Specification()
        {
            return View();
        }
        public async Task<ActionResult> Photo()
        {
            HtmlDocument doc = new HtmlDocument();
            using (HttpClient httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate }))
            {
                httpClient.BaseAddress = new Uri("https://privatbank.ua/");
                httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                httpClient.DefaultRequestHeaders.Add("accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36");
                string responseString;
                try
                {
                    responseString = await httpClient.GetStringAsync("https://privatbank.ua/");
                    doc.LoadHtml(responseString);
                }
                catch
                { }
            }
            HtmlNode node = null;
            if (!String.IsNullOrEmpty(doc.Text))
                node = doc.GetElementbyId("USD_sell");
            
            if (node != null)
            {
                try
                {
                    double Price = double.Parse(node.InnerText.Replace("\n", "").Trim().Replace(".", ",")) * 55000;
                    ViewBag.Price = Price.ToString("C0", CultureInfo.CreateSpecificCulture("uk-UA")) + " (55000$)";
                }
                catch
                {
                    ViewBag.Price = "Нет доступа к ресурсу с курсом валют (55000$)";
                }
            }
            else
            {
                ViewBag.Price = "Нет доступа к ресурсу с курсом валют (55000$)";
            }
            return View();
        }

        public ActionResult Videos()
        {
            return View();
        }

        public ActionResult Sitemap()
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");
            List<string> sitemapNodes = new List<string>();

            sitemapNodes.Add(Url.RouteUrl("Default", new { controller = "Home", action = "Index" }, Url.RequestContext.HttpContext.Request.Url.Scheme));
            sitemapNodes.Add(Url.RouteUrl("Default", new { controller = "Home", action = "Design" }, Url.RequestContext.HttpContext.Request.Url.Scheme));
            sitemapNodes.Add(Url.RouteUrl("Default", new { controller = "Home", action = "Specification" }, Url.RequestContext.HttpContext.Request.Url.Scheme));
            sitemapNodes.Add(Url.RouteUrl("Default", new { controller = "Home", action = "Photo" }, Url.RequestContext.HttpContext.Request.Url.Scheme));
            sitemapNodes.Add(Url.RouteUrl("Default", new { controller = "Home", action = "Videos" }, Url.RequestContext.HttpContext.Request.Url.Scheme));


            foreach (string sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode)));
                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);
            
            return this.Content(document.ToString(), "text/xml", Encoding.UTF8);
        }

    }
}