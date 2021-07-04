using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace WebsiteCrawler.Services
{
    public static class SitemapCrawlerService
    {
        private static (string[], string[]) GetLinksFromSitemap(string uri)
        {
            string sitemapData = HttpService.GetFileDataByUri(uri);

            XmlDocument xml = new();
            xml.LoadXml(sitemapData);

            XmlNodeList nodesList = xml.GetElementsByTagName("loc");

            string[] links = nodesList.Cast<XmlElement>().Select(element => element.InnerText).ToArray();
            string[] sitemaps = links.Where(link => link.Contains("sitemap") && link.EndsWith("xml")).ToArray();
            links = links.Except(sitemaps).ToArray();

            return (links, sitemaps);
        }


        private static string[] GetSitemapLinksFromRobots(string uri)
        {
            string robotsData = HttpService.GetFileDataByUri($"{uri}robots.txt");

            Regex rule = new(@"Sitemap: (.*)\b");
            MatchCollection matchCollection = rule.Matches(robotsData);

            string[] sitemaps = matchCollection.Select(match => match.Groups[1].Value)
                                               .Where(link => link.EndsWith(".xml"))
                                               .ToArray();

            if (sitemaps.Length == 0)
                throw new Exception("Sitemap files not found");

            return sitemaps;
        }
    }
}
