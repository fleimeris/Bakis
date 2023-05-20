using System.Text.RegularExpressions;
using FluentValidation;
using RestAPI.Domain.Data.Enums;

namespace RestAPI.Controllers.AuditRuleController.Requests;

public class InsertRuleRequest
{
    public string? Identifier { get; set; }
    public CookieCategory? Category { get; set; }
}

public class InsertRuleRequestValidator : AbstractValidator<InsertRuleRequest>
{
    public InsertRuleRequestValidator()
    {
        RuleFor(x => x.Identifier)
            .NotNull().WithMessage("Identifier cannot be null")
            .NotEmpty().WithMessage("Identifier cannot be empty")
            .Must(IsRegexValid).WithMessage("Regular expression is not valid");

        RuleFor(x => x.Category)
            .NotNull().WithMessage("Cookie category cannot be null")
            .NotEmpty().WithMessage("Cookie category cannot be empty")
            .IsInEnum().WithMessage("Cookie category is invalid");
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