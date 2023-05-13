using FluentValidation;
using PuppeteerSharp;
using RestAPI.Controllers.AuditRuleController.Requests;
using RestAPI.Controllers.ScanController.Requests;
using RestAPI.Domain.Services.RuleService;
using RestAPI.Domain.Services.ScannerService;

namespace RestAPI.Domain.Tests;

public class UnitTests
{
    [Fact]
    public void InsertsRule()
    {
        var ruleService = new RuleService();

        var text = Guid.NewGuid().ToString();
        
        ruleService.Insert(text, text);
        
        Assert.Contains(ruleService.GetAll(), x => x.Identifier == text && x.OnSuccess == text);
    }

    [Fact]
    public void DeleteRule()
    {
        var ruleService = new RuleService();

        var text = Guid.NewGuid().ToString();
        
        var ruleId = ruleService.Insert(text, text);
        
        Assert.NotNull(ruleService.GetById(ruleId));

        ruleService.Delete(ruleId);
        
        Assert.Null(ruleService.GetById(ruleId));
    }

    [Fact]
    public void GetAllRules()
    {
        var ruleService = new RuleService();

        var allRules = ruleService.GetAll();
        
        Assert.NotEmpty(allRules);
    }

    [Fact]
    public void UpdateRules()
    {
        var ruleService = new RuleService();

        var text = Guid.NewGuid().ToString();
        var newText = Guid.NewGuid().ToString();
        
        var ruleId = ruleService.Insert(text, text);

        ruleService.Update(ruleId, newText, newText);

        var rule = ruleService.GetById(ruleId);

        Assert.NotNull(rule);
        Assert.Equal(newText, rule.Identifier);
        Assert.Equal(newText, rule.OnSuccess);
    }

    [Fact]
    public void UpdateFakeRule()
    {
        var ruleService = new RuleService();
        
        ruleService.Update(Guid.Empty, string.Empty, string.Empty);
    }

    [Fact]
    public void ValidateGoodInsertRuleRequest()
    {
        var request = new InsertRuleRequest
        {
            Identifier = "test",
            OnSuccess = "test"
        };

        var requestValidator = new InsertRuleRequestValidator();

        var validationResult = requestValidator.Validate(request);
        
        Assert.True(validationResult.IsValid);
    }
    
    [Fact]
    public void ValidateBadInsertRuleRequest()
    {
        var request = new InsertRuleRequest
        {
            Identifier = "[",
            OnSuccess = "test"
        };

        var requestValidator = new InsertRuleRequestValidator();

        var validationResult = requestValidator.Validate(request);
        
        Assert.False(validationResult.IsValid);
    }

    [Fact]
    public void ValidateGoodUpdateRuleRequest()
    {
        var request = new UpdateRuleRequest
        {
            Identifier = "test",
            OnSuccess = "test"
        };

        var requestValidator = new UpdateRuleRequestValidator();
        
        var validationResult = requestValidator.Validate(request);
        
        Assert.True(validationResult.IsValid);
    }
    
    [Fact]
    public void ValidateBadUpdateRuleRequest()
    {
        var request = new UpdateRuleRequest
        {
            Identifier = "[",
            OnSuccess = "test"
        };

        var requestValidator = new UpdateRuleRequestValidator();
        
        var validationResult = requestValidator.Validate(request);
        
        Assert.False(validationResult.IsValid);
    }

    [Fact]
    public void ValidateGoodScanWebsiteRequest()
    {
        var request = new ScanWebsiteRequest
        {
            WebsiteUrl = "https://google.lt"
        };

        var requestValidator = new ScanWebsiteRequestValidator();
        
        var validationResult = requestValidator.Validate(request);
        
        Assert.True(validationResult.IsValid);
    }
    
    [Fact]
    public void ValidateBadFtpScanWebsiteRequest()
    {
        var request = new ScanWebsiteRequest
        {
            WebsiteUrl = "ftp://google.lt"
        };

        var requestValidator = new ScanWebsiteRequestValidator();
        
        var validationResult = requestValidator.Validate(request);
        
        Assert.False(validationResult.IsValid);
    }
    
    [Fact]
    public void ValidateBadStringScanWebsiteRequest()
    {
        var request = new ScanWebsiteRequest
        {
            WebsiteUrl = null
        };

        var requestValidator = new ScanWebsiteRequestValidator();
        
        var validationResult = requestValidator.Validate(request);
        
        Assert.False(validationResult.IsValid);
    }
    
    [Fact]
    public void ValidateBadScanWebsiteRequest()
    {
        var request = new ScanWebsiteRequest
        {
            WebsiteUrl = "["
        };

        var requestValidator = new ScanWebsiteRequestValidator();
        
        var validationResult = requestValidator.Validate(request);
        
        Assert.False(validationResult.IsValid);
    }

    [Fact]
    public async Task RunScan()
    {
        using var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        browserFetcher.Dispose();
        var scanService = new ScannerService(new RuleService());

        var result = await scanService.ScanWebsite("chrome://crashes/");
    }
}