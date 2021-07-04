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
            string[] linksFromSitemap = Array.Empty<string>();

            try
            {
                 linksFromSitemap = SitemapCrawlerService.Crawl(inputedUri);
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("\nTiming:");
                foreach (var item in foundLinks)
                    Console.WriteLine($"{item.Key} {item.Value.Milliseconds}ms");                
            }

            string[] linksInSitemapNotInSite = linksFromSitemap.Except(foundLinks.Keys).ToArray();
            string[] linksInSiteNotInSitemap = foundLinks.Keys.Except(linksFromSitemap).ToArray();

            if (linksInSitemapNotInSite.Length > 0)
                foundLinks.CustomConcat(VisitLinksFromSitemap(linksInSitemapNotInSite));

            foundLinks = foundLinks.OrderBy(item => item.Value)
                                   .ToDictionary(item => item.Key, item => item.Value);
            
            Console.WriteLine("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site:");
            foreach (string link in linksInSitemapNotInSite)
                Console.WriteLine($"    {link}");

            Console.WriteLine("\nUrls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml:");
            foreach (string link in linksInSiteNotInSitemap)
                Console.WriteLine($"    {link}");

            Console.WriteLine("\nTiming:");
            foreach (var item in foundLinks)
                Console.WriteLine($"{item.Key} {item.Value.Milliseconds}ms");
        }

        private static Dictionary<string, TimeSpan> VisitLinksFromSitemap(string[] links)
        {
            Dictionary<string, TimeSpan> linkTimePairs = new();

            foreach (string link in links)
                linkTimePairs.Add(link, HttpService.GetPageResponseTimeByUri(link));

            return linkTimePairs;
        }
    }
}
