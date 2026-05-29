using OrderApp.Data.Interfaces;
using OrderApp.Data.Models;

namespace OrderApp.Data.Contexts;

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