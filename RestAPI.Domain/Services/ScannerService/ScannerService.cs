using PuppeteerSharp;
using RestAPI.Domain.Data.Models;

namespace RestAPI.Domain.Services.ScannerService;

public class ScannerService : IScannerService
{
    private readonly HashSet<string> _websitesToVisit = new();
    private readonly HashSet<string> _visitedWebsites = new();
    private readonly HashSet<Cookie> _capturedCookies = new();

    private Uri _websiteUri;

    public async Task ScanWebsite(string websiteUrl)
    {
        _websiteUri = new Uri(websiteUrl);
        
        using var browserFetcher = new BrowserFetcher();
        await using var browser = await Puppeteer.LaunchAsync(
            new LaunchOptions { Headless = true });

        await using var page = await browser.NewPageAsync();

        await RecursiveCrawl(page, websiteUrl);
    }

    private async Task RecursiveCrawl(IPage page, string url)
    {
        if (_visitedWebsites.Contains(url) || _visitedWebsites.Count > 50)
            return;

        try
        {
            await page.GoToAsync(url);
            await page.WaitForNavigationAsync(new NavigationOptions
            {
                Timeout = 5
            });
        }
        catch
        {
            // ignore
        }

        _websitesToVisit.Remove(url);
        _visitedWebsites.Add(url);

        var foundCookies = await page.Client.SendAsync<List<Cookie>>("Network.getAllCookies");
        foreach (var foundCookie in foundCookies)
            _capturedCookies.Add(foundCookie);
        
        //TODO: add rules

        var newUrlsToVisit = await GetAllPageUrls(page);

        foreach (var urlToVisit in newUrlsToVisit)
            await ScanWebsite(urlToVisit);
    }

    private async Task<List<string>> GetAllPageUrls(IPage page)
    {
        try
        {
            var allHrefs =
                await page.EvaluateExpressionAsync<List<string>>(
                    "Array.from(document.getElementsByTagName('a'), a => a.href)");

            var result = new List<string>();

            foreach (var href in allHrefs)
            {
                try
                {
                    if(_websitesToVisit.Contains(href) || _visitedWebsites.Contains(href))
                        continue;
                    
                    var newLinkUrl = new Uri(href);
                    
                    if(newLinkUrl.Scheme != Uri.UriSchemeHttp && newLinkUrl.Scheme != Uri.UriSchemeHttps)
                        continue;
                    
                    if(newLinkUrl.Authority.Contains(_websiteUri.Authority, StringComparison.OrdinalIgnoreCase))
                        result.Add(href);
                }
                catch
                {
                    // ignore
                }
            }

            return result;
        }
        catch
        {
            return new List<string>();
        }
    }
}