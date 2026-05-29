using Microsoft.Extensions.Options;
using OrderApp.Query.Configuration;

namespace OrderApp.Query.Helpers;

public class OrderClassificationHelper(IOptions<OrderClassificationConfiguration> orderClassificationConfiguration)
    : IOrderClassificationHelper
{
    private static readonly List<OrderClassificationTier> DefaultTiers =
    [
        new() { MinimumAmount = 0,   MaximumAmount = 49.99, Classification = "LOW" },
        new() { MinimumAmount = 50,  MaximumAmount = 99.99, Classification = "MEDIUM" },
        new() { MinimumAmount = 100, MaximumAmount = null,  Classification = "HIGH" },
    ];
    
    private readonly List<OrderClassificationTier> validTiers = IsValidConfig(orderClassificationConfiguration.Value)
        ? orderClassificationConfiguration.Value.Tiers
        : DefaultTiers;
    
    public string GetOrderClassification(double orderTotal)
    {
        var tier = validTiers.FirstOrDefault(orderClassificationTier =>
            orderTotal >= orderClassificationTier.MinimumAmount &&
            (orderClassificationTier.MaximumAmount == null || orderTotal <= orderClassificationTier.MaximumAmount));

        return tier?.Classification ?? "UNKNOWN";
    }
    
    private static bool IsValidConfig(OrderClassificationConfiguration config) =>
        config.Tiers is { Count: > 0 } &&
        config.Tiers.Count(t => t.MaximumAmount == null) <= 1;
}