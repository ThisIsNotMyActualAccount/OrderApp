namespace OrderApp.Api.DataTransferObjects;

/// <summary>
/// Holds all the data related to an order.
/// </summary>
/// <param name="CustomerId">The customer identifier.</param>
/// <param name="Items">The items in the order.</param>
/// <param name="Total">The total cost.</param>
/// <param name="Classification">The classification of the order.</param>
public sealed record OrderDetailsDto(string CustomerId, OrderItemsDto[] Items, decimal Total, string Classification);