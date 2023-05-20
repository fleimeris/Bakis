using System.Text.RegularExpressions;
using PuppeteerSharp;
using RestAPI.Domain.Data.Enums;
using RestAPI.Domain.Data.Models;
using RestAPI.Domain.Services.ScannerService.Dtos;

namespace RestAPI.Domain.Services.ScannerService;

public class ScannerService : IScannerService
{
    private readonly HashSet<string> _websitesToVisit = new();
    private readonly HashSet<string> _visitedWebsites = new();
    private readonly HashSet<Cookie> _capturedCookies = new();
    private readonly ScanResult _scanResult = new();

    private Uri _websiteUri;

    private readonly IRuleService _ruleService;

    public ScannerService(IRuleService ruleService)
    {
        _ruleService = ruleService;
    }

    public async Task<ScanResult> ScanWebsite(string websiteUrl)
    {
        _websiteUri = new Uri(websiteUrl);
        
        using var browserFetcher = new BrowserFetcher();
        await using var browser = await Puppeteer.LaunchAsync(
            new LaunchOptions { Headless = true });

        await using var page = await browser.NewPageAsync();
        
        var allRules = _ruleService.GetAll();

        await RecursiveCrawl(page, websiteUrl);

        await page.DisposeAsync();
        await browser.DisposeAsync();

        _scanResult.Cookies = _capturedCookies.ToList();

        foreach (var auditRule in allRules)
        {
            foreach (var capturedCookie in _capturedCookies)
            {
                if (Regex.Matches(capturedCookie.Name!, auditRule.Identifier!).Count > 0)
                {
                    capturedCookie.Category = auditRule.Category;
                }
            }
        }

        return _scanResult;
    }

    private async Task RecursiveCrawl(IPage page, string url)
    {
        if (_visitedWebsites.Contains(url) || _visitedWebsites.Count > 25)
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

        await page.WaitForTimeoutAsync(3000);

        _websitesToVisit.Remove(url);
        _visitedWebsites.Add(url);

        var foundCookies = await page.Client.SendAsync<NetworkCookiesDto>("Network.getAllCookies");
        foreach (var foundCookie in foundCookies.Cookies)
        {
            Console.WriteLine(foundCookie.Expires);
            _capturedCookies.Add(foundCookie);
        }

        var newUrlsToVisit = await GetAllPageUrls(page);

        foreach (var maybePolicyUrl in newUrlsToVisit)
        {
            if (maybePolicyUrl.Contains("privacy", StringComparison.OrdinalIgnoreCase) ||
                maybePolicyUrl.Contains("privatumo", StringComparison.OrdinalIgnoreCase))
            {
                _scanResult.Policies.Add(new Policy
                {
                    Url = maybePolicyUrl,
                    Type = PredictedPolicyType.PrivacyPolicy
                });
            } else if (maybePolicyUrl.Contains("cookie", StringComparison.OrdinalIgnoreCase) ||
                       maybePolicyUrl.Contains("slapuku", StringComparison.OrdinalIgnoreCase) ||
                       maybePolicyUrl.Contains("slapukas", StringComparison.OrdinalIgnoreCase))
            {
                _scanResult.Policies.Add(new Policy
                {
                    Type = PredictedPolicyType.CookiePolicy,
                    Url = maybePolicyUrl
                });
            }
        }

        foreach (var urlToVisit in newUrlsToVisit)
            await RecursiveCrawl(page, urlToVisit);
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