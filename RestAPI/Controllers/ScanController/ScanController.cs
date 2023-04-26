using Microsoft.AspNetCore.Mvc;
using RestAPI.Controllers.ScanController.Requests;
using RestAPI.Domain.Services;

namespace RestAPI.Controllers.ScanController;

public class ScanController : BaseController
{
    private readonly IScannerService _scannerService;

    public ScanController(IScannerService scannerService)
    {
        _scannerService = scannerService;
    }

    [HttpPost]
    public async Task<ActionResult> ScanWebsite([FromBody] ScanWebsiteRequest request)
    {
        var result = await _scannerService.ScanWebsite(request.WebsiteUrl!);

        return new OkObjectResult(result);
    }
}