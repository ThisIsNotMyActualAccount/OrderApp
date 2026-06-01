namespace OrderApp.Data.Models;

/// <summary>
/// The order table database model.
/// </summary>
public class OrderTable
{
    /// <summary>
    /// The customer identifier.
    /// </summary>
    public required string CustomerId { get; set; }
    
    /// <summary>
    /// The items included in the order.
    /// </summary>
    public required OrderItemsTable[] Items { get; set; }
    
    /// <summary>
    /// The total price of everything in the order.
    /// </summary>
    public decimal Total { get; set; }
    
    /// <summary>
    /// The classification of the order.
    /// </summary>
    public required string Classification { get; set; }
}