using System.Collections.Concurrent;
using OrderApp.Data.Models;

namespace OrderApp.Data;

/// <summary>
/// To be used as the shared data storage.
/// </summary>
public class OrdersRepository
{
    /// <summary>
    /// The next order identifier.
    /// </summary>
    private int nextOrderId = 0;
    
    /// <summary>
    /// The key for the dictionary is the order id, and the value is the order itself.
    /// </summary>
    public ConcurrentDictionary<int, OrderTable> Orders { get; } = new();
    
    /// <summary>
    /// Collects the next order identifier.
    /// </summary>
    /// <returns>The order identifier as an int.</returns>
    public int GetNextOrderId() => Interlocked.Increment(ref nextOrderId);
}