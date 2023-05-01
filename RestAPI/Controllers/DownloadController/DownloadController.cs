using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestAPI.Domain.Data.Models;

namespace RestAPI.Controllers.DownloadController;

public class DownloadController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Download([FromQuery] int what, [FromBody] ScanResult scanResult)
    {
        string contentString;
        string fileName;
        string contentType;
        
        if (what == 0) //CSV
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Cookie name, Cookie domain");
            foreach (var cookie in scanResult.Cookies)
                stringBuilder.AppendLine($"{cookie.Name},{cookie.Domain}");

            contentString = stringBuilder.ToString();
            fileName = "report.csv";
            contentType = "text/csv";
        } else if (what == 1) //JSON
        {
            contentString = JsonConvert.SerializeObject(scanResult);
            fileName = "report.json";
            contentType = "application/json";
        }
        else
        {
            return new BadRequestResult();
        }

        var contentBytes = Encoding.UTF8.GetBytes(contentString);

        return File(contentBytes, contentType, fileName);
    }
}