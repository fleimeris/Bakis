using RestAPI.Domain.Data.Models;

namespace RestAPI.Domain.Services;

public interface IScannerService
{
    Task<ScanResult> ScanWebsite(string websiteUrl);
}