using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddFusionCache();
        services.AddScoped<IOrderService, OrderService>();
        
        return services;
    }
}
