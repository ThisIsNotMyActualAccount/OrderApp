namespace OrderApp.Query.Configuration;

/// <summary>
/// Defines the tiers for each of the classification items.
/// </summary>
public class OrderClassificationTier
{
    /// <summary>
    /// The minimum value for this tier.
    /// </summary>
    public decimal MinimumAmount { get; set; }
    
    /// <summary>
    /// The maximum value for this tier, null if there is no maximum (i.e. this is the last tier).
    /// </summary>
    public decimal? MaximumAmount { get; set; }
    
    /// <summary>
    /// The name for the classification.
    /// </summary>
    public string Classification { get; set; } = null!;
}