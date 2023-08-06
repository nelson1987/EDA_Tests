using EDA.Api.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace EDA.Application.Configurations;
public static class ServiceCollections
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IContaService, ContaService>();
        return services;
    }
}
