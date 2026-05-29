using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using OrderApp.Data.Contexts;
using OrderApp.Data.Interfaces;

namespace OrderApp.Data.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddSingleton<OrdersRepository>();
        services.AddSingleton<IReadOrdersDb, ReadOrdersContext>();
        services.AddSingleton<IWriteOrdersDb, WriteOrdersContext>();

        return services;
    }
}