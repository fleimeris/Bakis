namespace RestAPI.Domain.Data.Models;

public class AuditRule
{
    public Guid Id { get; set; }
    public string? Identifier { get; set; }
    public string? OnSuccess { get; set; }
}