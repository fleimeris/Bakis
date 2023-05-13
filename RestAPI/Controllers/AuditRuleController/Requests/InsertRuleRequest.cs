using System.Text.RegularExpressions;
using FluentValidation;

namespace RestAPI.Controllers.AuditRuleController.Requests;

public class InsertRuleRequest
{
    public string? Identifier { get; set; }
    public string? OnSuccess { get; set; }
}

public class InsertRuleRequestValidator : AbstractValidator<InsertRuleRequest>
{
    public InsertRuleRequestValidator()
    {
        RuleFor(x => x.Identifier)
            .NotNull().WithMessage("Identifier cannot be null")
            .NotEmpty().WithMessage("Identifier cannot be empty")
            .Must(IsRegexValid).WithMessage("Regular expression is not valid");

        RuleFor(x => x.OnSuccess)
            .NotNull().WithMessage("On success message cannot be null")
            .NotEmpty().WithMessage("On success message cannot be empty");
    }

    private bool IsRegexValid(string? regex)
    {
        try
        {
            var _ = Regex.Match("", regex!);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}