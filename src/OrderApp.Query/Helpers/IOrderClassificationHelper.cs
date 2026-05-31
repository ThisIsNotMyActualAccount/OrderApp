namespace OrderApp.Query.Helpers;

/// <summary>
/// Offers helper methods for order classification.
/// </summary>
public interface IOrderClassificationHelper
{
    /// <summary>
    /// Gets the order classification from the total.
    /// </summary>
    /// <param name="orderTotal">The order total.</param>
    /// <returns>The order classification.</returns>
    string GetOrderClassification(double orderTotal);
}