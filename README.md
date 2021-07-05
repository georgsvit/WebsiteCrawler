# WebsiteCrawler
Console application that crawls all reachable pages on the website

## **Technologies**
- .NET 5
- [AngleSharp](https://github.com/AngleSharp/AngleSharp)

## **Setup and run**
To run this project, follow next steps:
1. Download project
2. Open terminal and go to the directory with the project
3. Run the following command to start app
```
dotnet run
```
4. Enter URI of the website you want to crawl

## **Repository content**
- [Program.cs](https://github.com/georgsvit/WebsiteCrawler/blob/master/Program.cs) - file that contains entry point to the program, handles user input, calls services' methods and outputs result to Console
- Extensions
    - [DictionaryExtension.cs](https://github.com/georgsvit/WebsiteCrawler/blob/master/Extensions/DictionaryExtension.cs) - class-extension that implements concatenation of two dictionaries
- Services
    - [HttpService.cs](https://github.com/georgsvit/WebsiteCrawler/blob/master/Services/HttpService.cs) - static class that performs all http-requests that can be used in program
    - [SitemapCrawlerService.cs](https://github.com/georgsvit/WebsiteCrawler/blob/master/Services/SitemapCrawlerService.cs) - static class that implements logic of receiving and processing sitemap files 
    - [WebPagesCrawlerService.cs](https://github.com/georgsvit/WebsiteCrawler/blob/master/Services/WebPagesCrawlerService.cs) - static class that implements logic of crawling website's pages 
