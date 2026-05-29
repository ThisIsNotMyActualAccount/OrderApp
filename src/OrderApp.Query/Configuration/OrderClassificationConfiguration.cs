namespace OrderApp.Query.Configuration;

/// <summary>
/// The configuration needed for the order classification.
/// </summary>
public class OrderClassificationConfiguration
{
    public List<OrderClassificationTier> Tiers { get; set; } = new();
}