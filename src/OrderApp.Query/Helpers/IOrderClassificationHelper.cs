namespace OrderApp.Query.Helpers;

public interface IOrderClassificationHelper
{
    /// <summary>
    /// Gets the order classification from the total.
    /// </summary>
    /// <param name="orderTotal">The order total.</param>
    /// <returns>The order classification.</returns>
    string GetOrderClassification(double orderTotal);
}