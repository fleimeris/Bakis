using RestAPI.Domain.Data.Enums;
using RestAPI.Domain.Data.Models;

namespace RestAPI.Domain.Services.RuleService;

public class RuleService : IRuleService
{
    private readonly List<AuditRule> _data = new();

    public RuleService()
    {
        _data.Add(new AuditRule
        {
            Id = Guid.NewGuid(),
            Identifier = "language",
            OnSuccess = "Found marketing cookies",
            Type = AuditRuleType.Cookie
        });
        _data.Add(new AuditRule
        {
            Id = Guid.NewGuid(),
            Identifier = "/html/body/app-root/div/app-window-view/div/app-cookie-consent",
            Type = AuditRuleType.Element,
            OnSuccess = "Cookie juosta rasta",
            OnFailed = "Cookie juosta nerasta"
        });
    }

    public Guid Insert(string identifier, AuditRuleType type, string? onSuccess = null, string? onFailed = null)
    {
        var id = Guid.NewGuid();
        var data = new AuditRule
        {
            Id = id,
            Identifier = identifier,
            Type = type,
            OnFailed = onFailed,
            OnSuccess = onSuccess
        };
        
        _data.Add(data);

        return id;
    }

    public void Delete(Guid ruleId)
    {
        var rule = _data.FirstOrDefault(x => x.Id == ruleId);

        if (rule != null)
            _data.Remove(rule);
    }

    public AuditRule? GetById(Guid ruleId)
    {
        return _data.FirstOrDefault(x => x.Id == ruleId);
    }

    public List<AuditRule> GetAll()
    {
        return _data;
    }
    
    public void Update(Guid ruleId, string identifier, string? onSuccess = null,
        string? onFailed = null)
    {
        var value = _data.FirstOrDefault(x => x.Id == ruleId);
        
        if(value == null)
            return;
        
        value.Identifier = identifier;
        value.OnSuccess = onSuccess;
        value.OnFailed = onFailed;
    }
}