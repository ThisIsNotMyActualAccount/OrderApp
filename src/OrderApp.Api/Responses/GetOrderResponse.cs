using OrderApp.Api.DataTransferObjects;

namespace OrderApp.Api.Responses;

/// <summary>
/// A response object for collecting an order.
/// </summary>
/// <param name="OrderDetails">The order details, null if there were no matches.</param>
public sealed record GetOrderResponse(OrderDetailsDto? OrderDetails);