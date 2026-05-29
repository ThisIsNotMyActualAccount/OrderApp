namespace OrderApp.Data.Models;

public class OrderItemsTable
{
    public required string Sku { get; set; }
    
    public required int Quantity { get; set; }
    
    public required double Price { get; set; }
}