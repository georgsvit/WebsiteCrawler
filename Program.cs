using System;
using System.Collections.Generic;
using System.Linq;
using WebsiteCrawler.Extensions;
using WebsiteCrawler.Services;

namespace WebsiteCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputedUri = Console.ReadLine();

            try
            {
                _ = new Uri(inputedUri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            Dictionary<string, TimeSpan> foundLinks = WebPagesCrawlerService.Crawl(inputedUri);
            int foundLinksCount = foundLinks.Count;
            IEnumerable<string> linksFromSitemap = SitemapCrawlerService.Crawl(inputedUri);
            
            string[] linksInSitemapNotInSite = linksFromSitemap.Except(foundLinks.Keys).ToArray();
            IEnumerable<string> linksInSiteNotInSitemap = foundLinks.Keys.Except(linksFromSitemap);

            if (linksInSitemapNotInSite.Any())
                foundLinks.CustomConcat(VisitLinksFromSitemap(linksInSitemapNotInSite));

            foundLinks = foundLinks.OrderBy(item => item.Value)
                                   .ToDictionary(item => item.Key, item => item.Value);


            PrintTitleAndTable("Urls FOUND IN SITEMAP.XML but not founded after crawling a web site:",
                               linksInSitemapNotInSite,
                               s => $"  {s}");

            PrintTitleAndTable("Urls FOUND BY CRAWLING THE WEBSITE but not in sitemap.xml:",
                               linksInSiteNotInSitemap,
                               s => $"  {s}");

            PrintTitleAndTable("Timing:",
                               foundLinks,
                               item => $"{item.Key} {item.Value.Milliseconds}ms");

            Console.WriteLine($"\nUrls (html documents) found after crawling a website: {foundLinksCount}");
            Console.WriteLine($"Urls found in sitemap: {linksFromSitemap.Count()}");
        }

        private static void PrintTitleAndTable<T>(string title, IEnumerable<T> collection, Func<T, string> printElement)
        {
            Console.WriteLine($"\n{title}");
            foreach (var item in collection)
                Console.WriteLine(printElement(item));
        }

        private static Dictionary<string, TimeSpan> VisitLinksFromSitemap(IEnumerable<string> links)
        {
            Dictionary<string, TimeSpan> linkTimePairs = new();

            foreach (string link in links)
                linkTimePairs.Add(link, HttpService.GetPageResponseTimeByUri(link));

            return linkTimePairs;
        }
    }
}
