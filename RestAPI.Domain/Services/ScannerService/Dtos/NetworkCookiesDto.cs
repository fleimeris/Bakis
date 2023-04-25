using RestAPI.Domain.Data.Models;

namespace RestAPI.Domain.Services.ScannerService.Dtos;

public class NetworkCookiesDto
{
    public List<Cookie> Cookies { get; set; } = new();
}