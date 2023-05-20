using RestAPI.Domain.Data.Enums;

namespace RestAPI.Domain.Data.Models;

public class AuditRule
{
    public Guid Id { get; set; }
    public string? Identifier { get; set; }
    public CookieCategory Category { get; set; }
}