using OrderApp.Api.DataTransferObjects;
using OrderApp.Data.Interfaces;

namespace OrderApp.Data.Contexts;

/// <summary>
/// A concrete implementation of the <see cref="IReadOrdersDb"/> interface.
/// </summary>
/// <param name="ordersRepository">The order repository.</param>
public class ReadOrdersContext(OrdersRepository ordersRepository) : IReadOrdersDb
{
    /// <inheritdoc />
    public Task<OrderDetailsDto?> GetOrder(int orderId)
    {
        if (!ordersRepository.Orders.TryGetValue(orderId, out var order))
        {
            return Task.FromResult<OrderDetailsDto?>(null);
        }
        
        var orderDetails = new OrderDetailsDto(
            order.CustomerId,
            order.Items.Select(orderItem => new OrderItemsDto(orderItem.Sku, orderItem.Quantity, orderItem.Price)).ToArray(),
            order.Total,
            order.Classification
        );
        
        return Task.FromResult<OrderDetailsDto?>(orderDetails);
    }
}