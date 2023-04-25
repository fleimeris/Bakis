using RestAPI.Domain.Data.Enums;
using RestAPI.Domain.Data.Models;

namespace RestAPI.Domain.Services;

public interface IRuleService
{
    Guid Insert(string identifier, AuditRuleType type, string? onSuccess = null, string? onFailed = null);
    void Delete(Guid ruleId);
    AuditRule? GetById(Guid ruleId);
    List<AuditRule> GetAll();

    void Update(Guid ruleId, string identifier, string? onSuccess = null,
        string? onFailed = null);
}