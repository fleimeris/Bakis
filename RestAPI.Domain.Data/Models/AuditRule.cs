using RestAPI.Domain.Data.Enums;

namespace RestAPI.Domain.Data.Models;

public class AuditRule
{
    public string? Identifier { get; set; }
    public AuditRuleType Type { get; set; }
    public string? OnSuccess { get; set; }
    public string? OnFailed { get; set; }
}