using FluentValidation;
using RestAPI.Domain.Data.Enums;

namespace RestAPI.Controllers.AuditRuleController.Requests;

public class UpdateRuleRequest
{
    public string? Identifier { get; set; }
    public string? OnSuccess { get; set; }
    public string? OnFailed { get; set; }
}

public class UpdateRuleRequestValidator : AbstractValidator<UpdateRuleRequest>
{
    public UpdateRuleRequestValidator()
    {
        RuleFor(x => x.Identifier)
            .NotNull().WithMessage("Identifier cannot be null")
            .NotEmpty().WithMessage("Identifier cannot be empty");
        
        RuleFor(x => x.OnSuccess)
            .NotNull().WithMessage("On success message cannot be null")
            .NotEmpty().WithMessage("On success message cannot be empty");
        
        RuleFor(x => x.OnFailed)
            .NotNull().WithMessage("On failed message cannot be null")
            .NotEmpty().WithMessage("On failed message cannot be empty");
    }
}