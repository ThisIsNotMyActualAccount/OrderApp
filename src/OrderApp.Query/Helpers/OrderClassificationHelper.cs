using Microsoft.Extensions.Options;
using OrderApp.Query.Configuration;

namespace OrderApp.Query.Helpers;

/// <summary>
/// The concrete implementation of the <see cref="IOrderClassificationHelper"/> interface.
/// </summary>
/// <param name="orderClassificationConfiguration">The order classification configuration.</param>
public class OrderClassificationHelper(IOptions<OrderClassificationConfiguration> orderClassificationConfiguration)
    : IOrderClassificationHelper
{
    /// <summary>
    /// Default tiers that are used if the configuration is invalid.
    /// </summary>
    private static readonly List<OrderClassificationTier> DefaultTiers =
    [
        new() { MinimumAmount = 0,   MaximumAmount = 49.99M, Classification = "LOW" },
        new() { MinimumAmount = 50,  MaximumAmount = 100, Classification = "MEDIUM" },
        new() { MinimumAmount = 100.01M, MaximumAmount = null,  Classification = "HIGH" },
    ];
    
    private readonly List<OrderClassificationTier> validTiers = IsValidConfig(orderClassificationConfiguration.Value)
        ? orderClassificationConfiguration.Value.Tiers
        : DefaultTiers;
    
    public string GetOrderClassification(decimal orderTotal)
    {
        var tier = validTiers.FirstOrDefault(orderClassificationTier =>
            orderTotal >= orderClassificationTier.MinimumAmount &&
            (orderClassificationTier.MaximumAmount == null || orderTotal <= orderClassificationTier.MaximumAmount));

        return tier?.Classification ?? "UNKNOWN";
    }
    
    /// <summary>
    /// Checks if the provided configuration is valid.
    /// A valid configuration must have at least one tier and can only have one tier with a null maximum amount (indicating no upper limit).
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <returns>A boolean determining if the configuration is valid.</returns>
    private static bool IsValidConfig(OrderClassificationConfiguration config) =>
        config.Tiers is { Count: > 0 } &&
        config.Tiers.Count(t => t.MaximumAmount == null) <= 1;
}