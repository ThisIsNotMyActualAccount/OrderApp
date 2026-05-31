using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderApp.Api.Requests;
using OrderApp.Query.Configuration;
using OrderApp.Query.Helpers;

namespace OrderApp.Query.Extensions;

/// <summary>
/// Extension methods to be used with the setup of the service.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    /// <summary>
    /// Adds the services required by the query service.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <returns>The updated services collection.</returns>
    public static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddSingleton<IOrderClassificationHelper, OrderClassificationHelper>();
        services.AddScoped<IOrderClassificationHelper, OrderClassificationHelper>();

        return services;
    }

    /// <summary>
    /// Registers the configuration needed for the query service.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The updated services collection.</returns>
    public static IServiceCollection RegisterConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OrderClassificationConfiguration>(configuration.GetSection("OrderClassification"));

        return services;
    }
    
    /// <summary>
    /// Registers the minimal API endpoints for the query service.
    /// </summary>
    /// <param name="webApplication">The web application.</param>
    /// <returns>The updated web application.</returns>
    public static WebApplication RegisterMinimalApiEndpoints(this WebApplication webApplication)
    {
        webApplication.MapPost("/orders", async (IMediator mediatr, [FromBody]CreateOrderRequest request) =>
            {
                var createOrderResponse = await mediatr.Send(request).ConfigureAwait(false);
        
                return !string.IsNullOrWhiteSpace(createOrderResponse.OrderId)
                    ? Results.Created($"/orders/{createOrderResponse.OrderId}", createOrderResponse)
                    : Results.Problem("An error occurred while creating the order.");
            })
            .WithName("CreateOrder");

        webApplication.MapGet("/orders/{orderId}", async (IMediator mediatr, string orderId) =>
            {
                if (!int.TryParse(orderId, out var orderIdAsInt))
                {
                    return Results.BadRequest($"Order ID '{orderId}' is not valid.");
                }
        
                var getOrderResponse = await mediatr.Send(new GetOrderRequest(orderIdAsInt)).ConfigureAwait(false);
        
                return getOrderResponse.OrderDetails is not null
                    ? Results.Ok(getOrderResponse)
                    : Results.NotFound($"Order {orderId} was not found.");
            })
            .WithName("GetOrder");

        return webApplication;
    }
}