namespace RestAPI.Domain.Services;

public interface IScannerService
{
    Task ScanWebsite(string websiteUrl);
}