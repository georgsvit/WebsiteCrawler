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

            if (pathAndQuery is not null && pathAndQuery != "/")
            {
                links.Add(uri.Replace(pathAndQuery, ""), new());
            }

            string currentLink = uri;

            while (currentLink != null)
            {
                var (extractedLinks, responseTime) = ProcessPage(currentLink);

                links[currentLink] = responseTime;
                links.CustomConcat(extractedLinks);

                currentLink = links.FirstOrDefault(item => item.Value.TotalMilliseconds == 0).Key;
            }

            return links;
        }

        private static (Dictionary<string, TimeSpan>, TimeSpan) ProcessPage(string uri)
        {
            string host = uri.Substring(0, uri.IndexOf('/', 8));
            var (pageData, responseTime) = HttpService.GetFileDataAndResponseTimeByUri(uri);

            var links = GetLinksFromWebPage(pageData, host);

            return (links, responseTime);
        }

        private static Dictionary<string, TimeSpan> GetLinksFromWebPage(string pageData, string host)
        {
            HtmlParser parser = new();
            var page = parser.ParseDocument(pageData);

            var links = page.QuerySelectorAll("a")
                            .Select(element => element.GetAttribute("href"))
                            .Where(link => link is not null && (link.StartsWith(host) || link.StartsWith("/")) && !link.Contains("#") && !link.Contains("@"))
                            .Select(link => link.StartsWith("/") ? $"{host}{link}" : link)
                            .Distinct().ToDictionary(x => x, x => new TimeSpan());

            return links;
        }

        private static Dictionary<string, TimeSpan> GetLinksFromWebPageRegex(string pageData, string host)
        {
            Regex rule = new(@"(?inx)
                <a \s [^>]*
                    href \s* = \s*
                        (?<q> ['""] )
                            (?<url> [^""]+ )
                        \k<q>
                [^>]* >"
            );

            MatchCollection matchCollection = rule.Matches(pageData);

            var links = matchCollection.Select(match => match.Groups[2].ToString())
                            .Where(link => link is not null && (link.StartsWith(host) || link.StartsWith("/")) && !link.Contains("#") && !link.Contains("@"))
                            .Select(link => link.StartsWith("/") ? $"{host}{link}" : link)
                            .Distinct().ToDictionary(x => x, x => new TimeSpan());

            return links;
        }
    }
}
