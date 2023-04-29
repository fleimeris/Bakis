using FluentValidation;

namespace RestAPI.Controllers.ScanController.Requests;

public class ScanWebsiteRequest
{
    public string? WebsiteUrl { get; set; }
}

public class ScanWebsiteRequestValidator : AbstractValidator<ScanWebsiteRequest>
{
    public ScanWebsiteRequestValidator()
    {
        RuleFor(x => x.WebsiteUrl)
            .NotNull().WithMessage("Website url cannot be null")
            .NotEmpty().WithMessage("Website url cannot be empty")
            .Must(IsUrlValid).WithMessage("Website url is not valid");
    }

    private bool IsUrlValid(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        try
        {
            var uri = new Uri(url);
            
            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                return true;
        }
        catch
        {
            return false;
        }

        return false;
    }
}