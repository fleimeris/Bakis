namespace RestAPI.Domain.Data.Models;

public class Cookie
{
    public string? Name { get; set; }
    public string? Domain { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Cookie cookie)
            return true;
        
        return Name == cookie.Name;
    }

// -1250476639
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}