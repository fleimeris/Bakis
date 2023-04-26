using RestAPI.Domain.Data.Enums;

namespace RestAPI.Controllers.AuditRuleController.Responses;

public class GetAllRulesResponse
{
    public Guid Id { get; set; }
    public string? Identifier { get; set; }
    public AuditRuleType Type { get; set; }
    public string? OnSuccess { get; set; }
    public string? OnFailed { get; set; }
}