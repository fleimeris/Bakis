namespace RestAPI.Controllers.AuditRuleController.Responses;

public class GetAllRulesResponse
{
    public Guid Id { get; set; }
    public string? Identifier { get; set; }
    public string? OnSuccess { get; set; }
    public string? OnFailed { get; set; }
}