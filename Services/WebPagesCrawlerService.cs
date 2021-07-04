using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebsiteCrawler.Extensions;

namespace WebsiteCrawler.Services
{
    public static class WebPagesCrawlerService
    {
        public static Dictionary<string, TimeSpan> Crawl(string uri)
        {
            Dictionary<string, TimeSpan> links = new()
            {
                [uri] = new()
            };

            string pathAndQuery = new Uri(uri).PathAndQuery;

            if (pathAndQuery != "/")
            {
                links.Add(uri.Replace(pathAndQuery, ""), new());
            }

            string currentLink = uri;

            while (currentLink != null)
            {
                try
                {
                    var (extractedLinks, responseTime) = ProcessPage(currentLink);
                    links[currentLink] = responseTime;
                    links.CustomConcat(extractedLinks);
                }
                catch (Exception e)
                {
                    links[currentLink] = TimeSpan.MaxValue;

                    // Better to change to logger
                    Console.WriteLine($"URI: {currentLink} Message: {e.Message}");
                }

                currentLink = links.FirstOrDefault(item => item.Value.TotalMilliseconds == 0).Key;
            }

            return links;
        }

        private static (Dictionary<string, TimeSpan>, TimeSpan) ProcessPage(string uri)
        {            
            var (pageData, responseTime) = HttpService.GetFileDataAndResponseTimeByUri(uri);

            string host = $"https://{new Uri(uri).Host}";
            var links = GetLinksFromWebPage(pageData, host);

            return (links.ToDictionary(x => x, _ => new TimeSpan()), responseTime);
        }

        private static IEnumerable<string> GetLinksFromWebPage(string pageData, string host)
        {
            HtmlParser parser = new();
            var page = parser.ParseDocument(pageData);

            var links = page.QuerySelectorAll("a")
                            .Select(element => element.GetAttribute("href"))
                            .Where(link => link is not null && (link.StartsWith(host) || (link.StartsWith("/") && !link.StartsWith("//"))) && !link.Contains("#") && !link.Contains("@"))
                            .Select(link =>
                                link[0] switch
                                {
                                    '/' or '.' => $"{host}{link}",
                                    _ => link
                                }
                            ).Distinct();

            return links;
        }        
    }
}
