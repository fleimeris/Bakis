using System.ComponentModel.DataAnnotations;

namespace Frontend.CustomValidators;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class UrlValidator : ValidationAttribute
{
    public UrlValidator(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public override bool IsValid(object? value)
    {
        if (value is not string url)
            return false;

        try
        {
            var uri = new Uri(url);

            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }
        catch
        {
            return false;
        }
    }
}