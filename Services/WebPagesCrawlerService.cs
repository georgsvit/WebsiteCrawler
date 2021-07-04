using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.Services
{
    public static class WebPagesCrawlerService
    {
        private static Dictionary<string, TimeSpan> GetLinksFromWebPage(string pageData, string host)
        {
            HtmlParser parser = new();
            var page = parser.ParseDocument(pageData);

            var links = page.QuerySelectorAll("a")
                            .Select(element => element.GetAttribute("href"))
                            .Where(link => link is not null && (link.StartsWith(host) || link.StartsWith("/")) && !link.Contains("#"))
                            .Select(link => link.StartsWith("/") ? $"{host}{link}" : link)
                            .Distinct().ToDictionary(x => x, x => new TimeSpan());

            return links;
        }
    }
}
