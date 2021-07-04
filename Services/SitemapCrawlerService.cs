using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebsiteCrawler.Services
{
    public static class SitemapCrawlerService
    {
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
