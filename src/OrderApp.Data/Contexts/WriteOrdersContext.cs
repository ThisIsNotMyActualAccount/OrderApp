using OrderApp.Data.Interfaces;
using OrderApp.Data.Models;

namespace OrderApp.Data.Contexts;

/// <summary>
/// A concrete implementation of the <see cref="IWriteOrdersDb"/> interface.
/// </summary>
/// <param name="ordersRepository">The order repository.</param>
public class WriteOrdersContext(OrdersRepository ordersRepository) : IWriteOrdersDb
{
    /// <inheritdoc />
    public Task<int> CreateOrder(OrderTable order)
    {
        var orderId = ordersRepository.GetNextOrderId();
        
        ordersRepository.Orders.TryAdd(orderId, order);

        return Task.FromResult(orderId);
    }
}