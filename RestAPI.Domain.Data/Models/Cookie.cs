namespace RestAPI.Domain.Data.Models;

public class Cookie
{
    public string? Name { get; set; }
    public string? Domain { get; set; }
    public string? Path { get; set; }
    public float? Expires { get; set; }
    public bool Session { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Cookie cookie)
            return true;
        
        return Name == cookie.Name;
    }
    
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}