using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace WebsiteCrawler.Services
{
    public static class SitemapCrawlerService
    {
        public static IEnumerable<string> Crawl(string uri)
        {
            Queue<string> sitemapLinks = new(GetSitemapLinksFromRobots(uri));
            List<string> pageLinks = new();

            while (sitemapLinks.Any())
            {
                string currentLink = sitemapLinks.Dequeue();
                var (links, sitemaps) = GetLinksFromSitemap(currentLink);

                if (sitemaps.Any())
                    foreach (string sitemap in sitemaps)
                        sitemapLinks.Enqueue(sitemap);

                if (links.Any())
                    pageLinks.AddRange(links);
            }

            return pageLinks;
        }

        private static IEnumerable<string> GetSitemapLinksFromRobots(string uri)
        {
            string robotsData = HttpService.GetFileDataByUri($"https://{new Uri(uri).Host}/robots.txt");

            Regex rule = new(@"Sitemap: (.*\.xml)\b");
            MatchCollection matchCollection = rule.Matches(robotsData);

            return matchCollection.Select(match => match.Groups[1].Value);
        }

        private static (IEnumerable<string>, IEnumerable<string>) GetLinksFromSitemap(string uri)
        {
            string sitemapData = HttpService.GetFileDataByUri(uri);

            XmlDocument xml = new();
            xml.LoadXml(sitemapData);

            XmlNodeList nodesList = xml.GetElementsByTagName("loc");

            IEnumerable<string> links = nodesList.Cast<XmlElement>().Select(element => element.InnerText);
            IEnumerable<string> sitemaps = links.Where(link => link.Contains("sitemap") && link.EndsWith("xml"));
            links = links.Except(sitemaps);

            return (links, sitemaps);
        }
    }
}
