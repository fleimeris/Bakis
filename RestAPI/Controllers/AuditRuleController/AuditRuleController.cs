using Microsoft.AspNetCore.Mvc;
using RestAPI.Controllers.AuditRuleController.Requests;
using RestAPI.Controllers.AuditRuleController.Responses;
using RestAPI.Domain.Services;

namespace RestAPI.Controllers.AuditRuleController;

public class AuditRuleController : BaseController
{
    private readonly IRuleService _ruleService;

    public AuditRuleController(IRuleService ruleService)
    {
        _ruleService = ruleService;
    }

    [HttpPut]
    public IActionResult InsertRule([FromBody] InsertRuleRequest request)
    {
        _ruleService.Insert(request.Identifier!, request.Type!.Value, request.OnSuccess, request.OnFailed);
        
        return new OkResult();
    }

    [HttpDelete("{ruleId}")]
    public IActionResult DeleteRule([FromRoute] Guid ruleId)
    {
        var rule = _ruleService.GetById(ruleId);

        if (rule == null)
            return new NotFoundResult();
        
        _ruleService.Delete(ruleId);

        return new OkResult();
    }

    [HttpGet]
    public ActionResult<List<GetAllRulesResponse>> GetAllRules()
    {
        var rules = _ruleService.GetAll();
        
        return new OkObjectResult(rules.Select(x => new GetAllRulesResponse
        {
            Id = x.Id,
            Identifier = x.Identifier,
            OnSuccess = x.OnSuccess,
            OnFailed = x.OnFailed,
            Type = x.Type
        }));
    }

    [HttpPost("{ruleId}")]
    public IActionResult UpdateRule([FromRoute] Guid ruleId, [FromBody] UpdateRuleRequest request)
    {
        var rule = _ruleService.GetById(ruleId);

        if (rule == null)
            return new NotFoundResult();

        _ruleService.Update(ruleId, request.Identifier!, request.OnSuccess, request.OnFailed);

        return new OkResult();
    }
}