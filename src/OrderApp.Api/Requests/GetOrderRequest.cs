using MediatR;
using OrderApp.Api.Responses;

namespace OrderApp.Api.Requests;

/// <summary>
/// The request for creating an order.
/// </summary>
/// <param name="OrderId">The order identifier.</param>
public sealed record GetOrderRequest(int OrderId): IRequest<GetOrderResponse>;