using Microsoft.Extensions.Options;
using OrderApp.Query.Configuration;
using OrderApp.Query.Helpers;

namespace OrderApp.Query.Tests;

public class OrderClassificationHelperTests
{
    private OrderClassificationHelper _helper = null!;

    [SetUp]
    public void Setup()
    {
        // Use default configuration
        var options = Options.Create(new OrderClassificationConfiguration { Tiers = new List<OrderClassificationTier>() });
        _helper = new OrderClassificationHelper(options);
    }

    [Test]
    public void GetOrderClassification_WithLowTotal_ReturnsLow()
    {
        // Act
        var result = _helper.GetOrderClassification(25.00);

        // Assert
        Assert.That(result, Is.EqualTo("LOW"));
    }

    [Test]
    public void GetOrderClassification_WithMediumTotal_ReturnsMedium()
    {
        // Act
        var result = _helper.GetOrderClassification(75.00);

        // Assert
        Assert.That(result, Is.EqualTo("MEDIUM"));
    }

    [Test]
    public void GetOrderClassification_WithHighTotal_ReturnsHigh()
    {
        // Act
        var result = _helper.GetOrderClassification(150.00);

        // Assert
        Assert.That(result, Is.EqualTo("HIGH"));
    }

    [Test]
    public void GetOrderClassification_AtBoundary_LowMax_ReturnsLow()
    {
        // Arrange - 49.99 is the max for LOW
        const double total = 49.99;

        // Act
        var result = _helper.GetOrderClassification(total);

        // Assert
        Assert.That(result, Is.EqualTo("LOW"));
    }

    [Test]
    public void GetOrderClassification_AtBoundary_MediumMin_ReturnsMedium()
    {
        // Arrange - 50 is the min for MEDIUM
        const double total = 50.00;

        // Act
        var result = _helper.GetOrderClassification(total);

        // Assert
        Assert.That(result, Is.EqualTo("MEDIUM"));
    }

    [Test]
    public void GetOrderClassification_AtBoundary_MediumMax_ReturnsMedium()
    {
        // Arrange - 99.99 is the max for MEDIUM in defaults
        const double total = 99.99;

        // Act
        var result = _helper.GetOrderClassification(total);

        // Assert
        Assert.That(result, Is.EqualTo("MEDIUM"));
    }

    [Test]
    public void GetOrderClassification_AtBoundary_HighMin_ReturnsHigh()
    {
        // Arrange - 100.01 is the min for HIGH in defaults
        const double total = 100.01;

        // Act
        var result = _helper.GetOrderClassification(total);

        // Assert
        Assert.That(result, Is.EqualTo("HIGH"));
    }

    [Test]
    public void GetOrderClassification_WithZero_ReturnsLow()
    {
        // Act
        var result = _helper.GetOrderClassification(0);

        // Assert
        Assert.That(result, Is.EqualTo("LOW"));
    }

    [Test]
    public void GetOrderClassification_WithVeryLargeAmount_ReturnsHigh()
    {
        // Act
        var result = _helper.GetOrderClassification(999999.99);

        // Assert
        Assert.That(result, Is.EqualTo("HIGH"));
    }

    [Test]
    public void GetOrderClassification_WithInvalidConfig_UsesDefaults()
    {
        // Arrange - Create a config with two unbounded tiers (invalid)
        var invalidConfig = new OrderClassificationConfiguration
        {
            Tiers =
            [
                new OrderClassificationTier { MinimumAmount = 0, MaximumAmount = 50, Classification = "LOW" },
                new OrderClassificationTier { MinimumAmount = 50, MaximumAmount = null, Classification = "MEDIUM" },
                new OrderClassificationTier { MinimumAmount = 100, MaximumAmount = null, Classification = "HIGH" }
            ]
        };
        var options = Options.Create(invalidConfig);
        var helper = new OrderClassificationHelper(options);

        // Act
        var result = helper.GetOrderClassification(75.00);

        // Assert - Should fall back to defaults
        Assert.That(result, Is.EqualTo("MEDIUM"));
    }

    [Test]
    public void GetOrderClassification_WithEmptyConfig_UsesDefaults()
    {
        // Arrange - Create a config with no tiers
        var emptyConfig = new OrderClassificationConfiguration
        {
            Tiers = new List<OrderClassificationTier>()
        };
        var options = Options.Create(emptyConfig);
        var helper = new OrderClassificationHelper(options);

        // Act
        var result = helper.GetOrderClassification(75.00);

        // Assert - Should fall back to defaults
        Assert.That(result, Is.EqualTo("MEDIUM"));
    }

    [Test]
    public void GetOrderClassification_WithCustomValidConfig_UsesCustom()
    {
        // Arrange - Create a custom valid config
        var customConfig = new OrderClassificationConfiguration
        {
            Tiers =
            [
                new OrderClassificationTier { MinimumAmount = 0, MaximumAmount = 100, Classification = "BASIC" },
                new OrderClassificationTier { MinimumAmount = 100.01, MaximumAmount = null, Classification = "PREMIUM" }
            ]
        };
        var options = Options.Create(customConfig);
        var helper = new OrderClassificationHelper(options);

        // Act
        var lowResult = helper.GetOrderClassification(50.00);
        var highResult = helper.GetOrderClassification(150.00);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(lowResult, Is.EqualTo("BASIC"));
            Assert.That(highResult, Is.EqualTo("PREMIUM"));
        });
    }

    [Test]
    public void GetOrderClassification_WithNoMatchingTier_ReturnsUnknown()
    {
        // Arrange - This shouldn't happen with the default config, but test the fallback
        var options = Options.Create(new OrderClassificationConfiguration 
        { 
            Tiers = [new OrderClassificationTier { MinimumAmount = 500, MaximumAmount = 1000, Classification = "HUGE" }]
        });
        var helper = new OrderClassificationHelper(options);

        // Act
        var result = helper.GetOrderClassification(75.00); // Doesn't match any tier

        // Assert
        Assert.That(result, Is.EqualTo("UNKNOWN"));
    }

    [Test]
    [TestCase(0.01, "LOW")]
    [TestCase(25.50, "LOW")]
    [TestCase(49.99, "LOW")]
    [TestCase(50.00, "MEDIUM")]
    [TestCase(75.00, "MEDIUM")]
    [TestCase(100.00, "MEDIUM")]
    [TestCase(100.01, "HIGH")]
    [TestCase(500.00, "HIGH")]
    [TestCase(1000.00, "HIGH")]
    public void GetOrderClassification_VariousAmounts_ReturnsCorrectClassification(double total, string expected)
    {
        // Act
        var result = _helper.GetOrderClassification(total);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}



