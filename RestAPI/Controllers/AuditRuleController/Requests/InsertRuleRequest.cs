using FluentValidation;
using RestAPI.Domain.Data.Enums;

namespace RestAPI.Controllers.AuditRuleController.Requests;

public class InsertRuleRequest
{
    public string? Identifier { get; set; }
    public AuditRuleType? Type { get; set; }
    public string? OnSuccess { get; set; }
    public string? OnFailed { get; set; }
}

public class InsertRuleRequestValidator : AbstractValidator<InsertRuleRequest>
{
    public InsertRuleRequestValidator()
    {
        RuleFor(x => x.Identifier)
            .NotNull().WithMessage("Identifier cannot be null")
            .NotEmpty().WithMessage("Identifier cannot be empty");

        RuleFor(x => x.Type)
            .NotNull().WithMessage("Rule type cannot be null")
            .IsInEnum().WithMessage("Rule type is invalid");

        RuleFor(x => x.OnSuccess)
            .NotNull().WithMessage("On success message cannot be null")
            .NotEmpty().WithMessage("On success message cannot be empty");
        
        RuleFor(x => x.OnFailed)
            .NotNull().WithMessage("On failed message cannot be null")
            .NotEmpty().WithMessage("On failed message cannot be empty");
    }
}