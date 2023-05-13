using RestAPI.Domain.Data.Models;

namespace RestAPI.Domain.Services;

public interface IRuleService
{
    Guid Insert(string identifier, string? onSuccess = null);
    void Delete(Guid ruleId);
    AuditRule? GetById(Guid ruleId);
    List<AuditRule> GetAll();
    void Update(Guid ruleId, string identifier, string? onSuccess = null);
}