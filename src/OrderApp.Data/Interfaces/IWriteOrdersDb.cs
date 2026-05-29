using OrderApp.Data.Models;

namespace OrderApp.Data.Interfaces;

public interface IWriteOrdersDb
{
    /// <summary>
    /// Creates an order in the database with the given properties.
    /// </summary>
    /// <param name="order">The order details.</param>
    /// <returns>An order id if the order was created successfully.</returns>
    Task<int> CreateOrder(OrderTable order);
}