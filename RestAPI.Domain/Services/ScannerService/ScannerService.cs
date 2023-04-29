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
        
        foreach (var auditRule in allRules.Where(x => x.Type == AuditRuleType.Element))
            _scanResult.RulesFound.Add(new RulesFound
            {
                Type = AuditRuleType.Element,
                RuleId = auditRule.Id,
                Rule = auditRule,
                Found = false
            });

        await RecursiveCrawl(page, websiteUrl);

        await page.DisposeAsync();
        await browser.DisposeAsync();

        _scanResult.Cookies = _capturedCookies.ToList();

        foreach (var auditRule in allRules.Where(x => x.Type == AuditRuleType.Cookie))
        {
            foreach (var capturedCookie in _capturedCookies)
            {
                if(Regex.Matches(capturedCookie.Name!, auditRule.Identifier!).Count > 0)
                    _scanResult.RulesFound.Add(new RulesFound
                    {
                        Cookie = capturedCookie,
                        Rule = auditRule,
                        RuleId = auditRule.Id,
                        Type = AuditRuleType.Cookie
                    });
            }
        }

        return _scanResult;
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

        var foundCookies = await page.Client.SendAsync<NetworkCookiesDto>("Network.getAllCookies");
        foreach (var foundCookie in foundCookies.Cookies)
            _capturedCookies.Add(foundCookie);

        foreach (var rulesFound in _scanResult.RulesFound.Where(x => x.Type == AuditRuleType.Element))
        {
            var elements = await page.XPathAsync(rulesFound.Rule!.Identifier);

            if (elements is { Length: > 0 })
                rulesFound.Found = true;
        }

        var newUrlsToVisit = await GetAllPageUrls(page);

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