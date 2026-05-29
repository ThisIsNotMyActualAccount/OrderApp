namespace OrderApp.Data.Models;

public class OrderTable
{
    public required string CustomerId { get; set; }
    
    public required OrderItemsTable[] Items { get; set; }
    
    public double Total { get; set; }
    
    public required string Classification { get; set; }
}