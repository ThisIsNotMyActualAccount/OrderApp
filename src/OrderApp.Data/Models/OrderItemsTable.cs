namespace OrderApp.Data.Models;

/// <summary>
/// The order items database model.
/// </summary>
public class OrderItemsTable
{
    /// <summary>
    /// The SKU of the item.
    /// </summary>
    public required string Sku { get; set; }
    
    /// <summary>
    /// The quantity of the item ordered.
    /// </summary>
    public required int Quantity { get; set; }
    
    /// <summary>
    /// The price of a single item.
    /// </summary>
    public required decimal Price { get; set; }
}