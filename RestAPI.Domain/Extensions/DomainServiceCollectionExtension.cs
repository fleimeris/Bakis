using Microsoft.Extensions.DependencyInjection;
using RestAPI.Domain.Services;
using RestAPI.Domain.Services.RuleService;
using RestAPI.Domain.Services.ScannerService;

namespace RestAPI.Domain.Extensions;

public static class DomainServiceCollectionExtension
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IScannerService, ScannerService>();
        services.AddSingleton<IRuleService, RuleService>();

        return services;
    }
}