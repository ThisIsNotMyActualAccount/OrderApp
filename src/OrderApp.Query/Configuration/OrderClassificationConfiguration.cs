namespace OrderApp.Query.Configuration;

/// <summary>
/// The configuration needed for the order classification.
/// </summary>
public class OrderClassificationConfiguration
{
    /// <summary>
    /// The tiers for the order classification.
    /// </summary>
    public List<OrderClassificationTier> Tiers { get; set; } = new();
}