using OrderApp.Api.DataTransferObjects;
using OrderApp.Data.Interfaces;

namespace OrderApp.Data.Contexts;

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
            order.Items.Select(i => new OrderItemsDto(i.Sku, i.Quantity, i.Price)).ToArray(),
            order.Total,
            order.Classification
        );
        
        return Task.FromResult<OrderDetailsDto?>(orderDetails);
    }
}