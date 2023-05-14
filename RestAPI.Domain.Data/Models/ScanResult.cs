namespace RestAPI.Domain.Data.Models;

public class ScanResult
{
    public List<Cookie> Cookies { get; set; } = new();
    public List<RulesFound> RulesFound { get; set; } = new();
    public HashSet<Policy> Policies { get; } = new();
}