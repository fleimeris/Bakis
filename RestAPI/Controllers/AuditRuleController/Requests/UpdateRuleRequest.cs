using RestAPI.Domain.Data.Enums;

namespace RestAPI.Controllers.AuditRuleController.Requests;

public class UpdateRuleRequest
{
    public string? Identifier { get; set; }
    public string? OnSuccess { get; set; }
    public string? OnFailed { get; set; }
}