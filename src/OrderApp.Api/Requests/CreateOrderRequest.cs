using System.ComponentModel.DataAnnotations;
using MediatR;
using OrderApp.Api.DataTransferObjects;
using OrderApp.Api.Responses;

namespace OrderApp.Api.Requests;

/// <summary>
/// The request for creating an order.
/// </summary>
/// <param name="CustomerId">The customer identifier.</param>
/// <param name="Items">The items included in the order.</param>
public sealed record CreateOrderRequest(
    [Required(ErrorMessage = "Customer identifier is required.")]
    [StringLength(100, ErrorMessage = "Customer identifier cannot exceed 100 characters.")] // Not sure the actual length
    string CustomerId,
    
    [Required(ErrorMessage = "Items are required.")]
    OrderItemsDto[] Items): IRequest<CreateOrderResponse>;