using System.Diagnostics.CodeAnalysis;
using OrderApp.Query.Configuration;
using OrderApp.Query.Helpers;

namespace OrderApp.Query.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddSingleton<IOrderClassificationHelper, OrderClassificationHelper>();
        services.AddScoped<IOrderClassificationHelper, OrderClassificationHelper>();

        return services;
    }

    public static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OrderClassificationConfiguration>(configuration.GetSection("OrderClassification"));

        return services;
    }
}