using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using OrderApp.Data.Contexts;
using OrderApp.Data.Interfaces;

namespace OrderApp.Data.Extensions;

/// <summary>
/// Extension methods to be used with the setup of the service.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    /// <summary>
    /// Adds the services required by the data layer.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <returns>The updated services collection.</returns>
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddSingleton<OrdersRepository>();
        services.AddSingleton<IReadOrdersDb, ReadOrdersContext>();
        services.AddSingleton<IWriteOrdersDb, WriteOrdersContext>();

        return services;
    }
}