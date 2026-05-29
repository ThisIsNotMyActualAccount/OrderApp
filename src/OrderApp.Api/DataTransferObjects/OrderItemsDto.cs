using System.ComponentModel.DataAnnotations;

namespace OrderApp.Api.DataTransferObjects;

/// <summary>
/// Contains all the items included in an order.
/// </summary>
/// <param name="Sku">The SKU of the item.</param>
/// <param name="Quantity">How many of the item are ordered.</param>
/// <param name="Price">The price of an individual item.</param>
public sealed record OrderItemsDto(
    [Required(ErrorMessage = "SKU is required.")]
    [StringLength(100, ErrorMessage = "SKU cannot exceed 100 characters.")] // Not sure the actual length
    string Sku,
    
    [Required(ErrorMessage = "Quantity is required.")]
    int Quantity,
    
    [Required(ErrorMessage = "Price is required.")]
    double Price);