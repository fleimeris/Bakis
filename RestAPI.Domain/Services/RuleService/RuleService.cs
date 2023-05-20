using Newtonsoft.Json;
using RestAPI.Domain.Data.Enums;
using RestAPI.Domain.Data.Models;

namespace RestAPI.Domain.Services.RuleService;

public class RuleService : IRuleService
{
    public Guid Insert(string identifier, CookieCategory category)
    {
        var id = Guid.NewGuid();
        var data = new AuditRule
        {
            Id = id,
            Identifier = identifier,
            Category = category
        };

        var rules = GetAll();
        rules.Add(data);
        
        File.WriteAllText("db.json", JsonConvert.SerializeObject(rules));

        return id;
    }

    public void Delete(Guid ruleId)
    {
        var rules = GetAll();
        
        var rule = rules.FirstOrDefault(x => x.Id == ruleId);

        if (rule != null)
            rules.Remove(rule);
        
        File.WriteAllText("db.json", JsonConvert.SerializeObject(rules));
    }

    public AuditRule? GetById(Guid ruleId)
    {
        var rules = GetAll();
        
        return rules.FirstOrDefault(x => x.Id == ruleId);
    }

    public List<AuditRule> GetAll()
    {
        var fileData = File.ReadAllText("db.json");
        var result = JsonConvert.DeserializeObject<List<AuditRule>>(fileData);

        return result ?? new List<AuditRule>();
    }

    public void Update(Guid ruleId, string identifier, CookieCategory category)
    {
        var rules = GetAll();
        
        var value = rules.FirstOrDefault(x => x.Id == ruleId);

        if (value == null)
            return;

        value.Identifier = identifier;
        value.Category = category;
    }
}