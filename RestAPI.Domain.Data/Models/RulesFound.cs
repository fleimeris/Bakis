namespace RestAPI.Domain.Data.Models;

public class RulesFound
{
    public Guid RuleId { get; set; }
    public AuditRule? Rule { get; set; }
    public Cookie? Cookie { get; set; }
}