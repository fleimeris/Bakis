using Microsoft.AspNetCore.Mvc;
using RestAPI.Domain.Services;

namespace RestAPI.Controllers.TestController;

public class TestController : BaseController
{
    private readonly IScannerService _scannerService;

    public TestController(IScannerService scannerService)
    {
        _scannerService = scannerService;
    }

    [HttpGet]
    public async Task<IActionResult> TestScanning()
    {
        await _scannerService.ScanWebsite("https://privacypartners.lt");
        
        return new OkResult();
    }
}