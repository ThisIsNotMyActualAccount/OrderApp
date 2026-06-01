namespace OrderApp.Api.Responses;

/// <summary>
/// A response object for creating an order.
/// </summary>
/// <param name="OrderId">The order identifier.</param>
/// <param name="Total">The total cost of the order.</param>
/// <param name="Classification">The classification of the order.</param>
public sealed record CreateOrderResponse(string? OrderId = null, decimal? Total = null, string? Classification = null);