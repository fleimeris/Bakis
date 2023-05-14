using RestAPI.Domain.Data.Enums;

namespace RestAPI.Domain.Data.Models;

public class Policy
{
    public string? Url { get; set; }
    public PredictedPolicyType Type { get; set; } 
    
    public override bool Equals(object? obj)
    {
        if (obj is not Policy policy)
            return true;
        
        return Url == policy.Url;
    }
    
    public override int GetHashCode()
    {
        return Url.GetHashCode();
    }
}