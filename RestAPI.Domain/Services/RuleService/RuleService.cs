using RestAPI.Domain.Data.Enums;
using RestAPI.Domain.Data.Models;

namespace RestAPI.Domain.Services.RuleService;

public class RuleService : IRuleService
{
    private readonly Dictionary<Guid, AuditRule> _data = new();

    public Guid Insert(string identifier, AuditRuleType type, string? onSuccess = null, string? onFailed = null)
    {
        var id = Guid.NewGuid();
        var data = new AuditRule
        {
            Identifier = identifier,
            Type = type,
            OnFailed = onFailed,
            OnSuccess = onSuccess
        };
        
        _data.Add(id, data);

        return id;
    }

    public void Delete(Guid ruleId)
    {
        if (_data.ContainsKey(ruleId))
            _data.Remove(ruleId);
    }

    public AuditRule? GetById(Guid ruleId)
    {
        return _data.TryGetValue(ruleId, out var value) ? value : null;
    }

    public List<AuditRule> GetAll()
    {
        return _data.Select(x => x.Value).ToList();
    }

    public void Update(Guid ruleId, string identifier, string? onSuccess = null,
        string? onFailed = null)
    {
        if (!_data.ContainsKey(ruleId))
            return;
        
        var value = _data[ruleId];
        value.Identifier = identifier;
        value.OnSuccess = onSuccess;
        value.OnFailed = onFailed;
    }
}