using OrderApp.Api.DataTransferObjects;

namespace OrderApp.Data.Interfaces;

public interface IReadOrdersDb
{
    /// <summary>
    /// Gets a specific order from the database.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <returns>A <see cref="OrderDetailsDto"/> with all the order details, null if no order matches.</returns>
    Task<OrderDetailsDto?> GetOrder(int orderId);
}