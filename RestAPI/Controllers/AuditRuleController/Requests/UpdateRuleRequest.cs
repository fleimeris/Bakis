using System.Text.RegularExpressions;
using FluentValidation;
using RestAPI.Domain.Data.Enums;

namespace RestAPI.Controllers.AuditRuleController.Requests;

public class UpdateRuleRequest
{
    public string? Identifier { get; set; }
    public CookieCategory? Category { get; set; }
}

public class UpdateRuleRequestValidator : AbstractValidator<UpdateRuleRequest>
{
    public UpdateRuleRequestValidator()
    {
        RuleFor(x => x.Identifier)
            .NotNull().WithMessage("Identifier cannot be null")
            .NotEmpty().WithMessage("Identifier cannot be empty")
            .Must(IsRegexValid).WithMessage("Regular expression is not valid");
        
        RuleFor(x => x.Category)
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