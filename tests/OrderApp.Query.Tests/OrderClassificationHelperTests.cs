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
        var result = _helper.GetOrderClassification(25.00m);

        // Assert
        Assert.That(result, Is.EqualTo("LOW"));
    }

    [Test]
    public void GetOrderClassification_WithMediumTotal_ReturnsMedium()
    {
        // Act
        var result = _helper.GetOrderClassification(75.00m);

        // Assert
        Assert.That(result, Is.EqualTo("MEDIUM"));
    }

    [Test]
    public void GetOrderClassification_WithHighTotal_ReturnsHigh()
    {
        // Act
        var result = _helper.GetOrderClassification(150.00m);

        // Assert
        Assert.That(result, Is.EqualTo("HIGH"));
    }

    [Test]
    public void GetOrderClassification_AtBoundary_LowMax_ReturnsLow()
    {
        // Arrange - 49.99 is the max for LOW
        const decimal total = 49.99m;

        // Act
        var result = _helper.GetOrderClassification(total);

        // Assert
        Assert.That(result, Is.EqualTo("LOW"));
    }

    [Test]
    public void GetOrderClassification_AtBoundary_MediumMin_ReturnsMedium()
    {
        // Arrange - 50 is the min for MEDIUM
        const decimal total = 50.00m;

        // Act
        var result = _helper.GetOrderClassification(total);

        // Assert
        Assert.That(result, Is.EqualTo("MEDIUM"));
    }

    [Test]
    public void GetOrderClassification_AtBoundary_MediumMax_ReturnsMedium()
    {
        // Arrange - 100 is the max for MEDIUM in defaults
        const decimal total = 100.00m;

        // Act
        var result = _helper.GetOrderClassification(total);

        // Assert
        Assert.That(result, Is.EqualTo("MEDIUM"));
    }

    [Test]
    public void GetOrderClassification_AtBoundary_HighMin_ReturnsHigh()
    {
        // Arrange - 100.01 is the min for HIGH in defaults
        const decimal total = 100.01m;

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
        var result = _helper.GetOrderClassification(999999.99m);

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
        var result = helper.GetOrderClassification(75.00m);

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
        var result = helper.GetOrderClassification(75.00m);

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
                new OrderClassificationTier { MinimumAmount = 100.01m, MaximumAmount = null, Classification = "PREMIUM" }
            ]
        };
        var options = Options.Create(customConfig);
        var helper = new OrderClassificationHelper(options);

        // Act
        var lowResult = helper.GetOrderClassification(50.00m);
        var highResult = helper.GetOrderClassification(150.00m);

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
        var result = helper.GetOrderClassification(75.00m); // Doesn't match any tier

        // Assert
        Assert.That(result, Is.EqualTo("UNKNOWN"));
    }

    private static readonly object[] ClassificationCases =
    [
        new object[] { 0.01m, "LOW" },
        new object[] { 25.50m, "LOW" },
        new object[] { 49.99m, "LOW" },
        new object[] { 50.00m, "MEDIUM" },
        new object[] { 75.00m, "MEDIUM" },
        new object[] { 100.00m, "MEDIUM" },
        new object[] { 100.01m, "HIGH" },
        new object[] { 500.00m, "HIGH" },
        new object[] { 1000.00m, "HIGH" }
    ];

    [TestCaseSource(nameof(ClassificationCases))]
    public void GetOrderClassification_VariousAmounts_ReturnsCorrectClassification(decimal total, string expected)
    {
        // Act
        var result = _helper.GetOrderClassification(total);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}



