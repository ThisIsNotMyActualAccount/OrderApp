using Microsoft.Extensions.Logging;
using Moq;
using OrderApp.Api.DataTransferObjects;
using OrderApp.Api.Requests;
using OrderApp.Data.Interfaces;
using OrderApp.Data.Models;
using OrderApp.Query.Handlers.Commands;
using OrderApp.Query.Helpers;

namespace OrderApp.Query.Tests;

public class CreateOrderRequestHandlerTests
{
    private Mock<ILogger<CreateOrderRequestHandler>> _mockLogger = null!;
    private Mock<IWriteOrdersDb> _mockWriteOrdersDb = null!;
    private Mock<IOrderClassificationHelper> _mockClassificationHelper = null!;
    private CreateOrderRequestHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<CreateOrderRequestHandler>>();
        _mockWriteOrdersDb = new Mock<IWriteOrdersDb>();
        _mockClassificationHelper = new Mock<IOrderClassificationHelper>();
        
        _handler = new CreateOrderRequestHandler(_mockLogger.Object, _mockWriteOrdersDb.Object, _mockClassificationHelper.Object);
    }

    [Test]
    public async Task Handle_WithValidRequest_ReturnsSuccessResponseWithOrderId()
    {
        // Arrange
        const string customerId = "CUST123";
        var items = new[]
        {
            new OrderItemsDto("SKU001", 2, 25.00m),
            new OrderItemsDto("SKU002", 1, 50.00m)
        };
        var request = new CreateOrderRequest(customerId, items);
        const int expectedOrderId = 1;
        const decimal expectedTotal = 100.00m;
        const string expectedClassification = "MEDIUM";

        _mockClassificationHelper.Setup(x => x.GetOrderClassification(expectedTotal))
            .Returns(expectedClassification);
        _mockWriteOrdersDb.Setup(x => x.CreateOrder(It.IsAny<OrderTable>()))
            .ReturnsAsync(expectedOrderId);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.OrderId, Is.EqualTo("0001"));
            Assert.That(result.Total, Is.EqualTo(expectedTotal));
            Assert.That(result.Classification, Is.EqualTo(expectedClassification));
        });

        _mockWriteOrdersDb.Verify(x => x.CreateOrder(It.IsAny<OrderTable>()), Times.Once);
        _mockClassificationHelper.Verify(x => x.GetOrderClassification(expectedTotal), Times.Once);
    }

    [Test]
    public async Task Handle_CalculatesCorrectTotal()
    {
        // Arrange
        const string customerId = "CUST123";
        var items = new[]
        {
            new OrderItemsDto("SKU001", 3, 10.50m),
            new OrderItemsDto("SKU002", 2, 15.75m)
        };
        var request = new CreateOrderRequest(customerId, items);

        _mockClassificationHelper.Setup(x => x.GetOrderClassification(It.IsAny<decimal>()))
            .Returns("MEDIUM");
        _mockWriteOrdersDb.Setup(x => x.CreateOrder(It.IsAny<OrderTable>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        // Verify CreateOrder was called once
        _mockWriteOrdersDb.Verify(
            x => x.CreateOrder(It.IsAny<OrderTable>()),
            Times.Once);
        
        // Capture the order passed to CreateOrder to verify the total
        _mockWriteOrdersDb.Verify(
            x => x.CreateOrder(It.Is<OrderTable>(o => 
                o.Items.Length == 2 &&
                o.Items[0].Sku == "SKU001" && o.Items[0].Quantity == 3 && o.Items[0].Price == 10.50m &&
                o.Items[1].Sku == "SKU002" && o.Items[1].Quantity == 2 && o.Items[1].Price == 15.75m)),
            Times.Once);
    }

    [Test]
    public async Task Handle_PassesCorrectOrderTableToRepository()
    {
        // Arrange
        const string customerId = "CUST456";
        var items = new[]
        {
            new OrderItemsDto("SKU001", 1, 25.00m)
        };
        var request = new CreateOrderRequest(customerId, items);

        _mockClassificationHelper.Setup(x => x.GetOrderClassification(It.IsAny<decimal>()))
            .Returns("LOW");
        _mockWriteOrdersDb.Setup(x => x.CreateOrder(It.IsAny<OrderTable>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockWriteOrdersDb.Verify(
            x => x.CreateOrder(It.Is<OrderTable>(o =>
                o.CustomerId == customerId &&
                o.Items.Length == 1 &&
                o.Items[0].Sku == "SKU001" &&
                o.Items[0].Quantity == 1 &&
                o.Items[0].Price == 25.00m &&
                o.Classification == "LOW")),
            Times.Once);
    }

    [Test]
    public async Task Handle_CallsClassificationHelperWithCorrectTotal()
    {
        // Arrange
        const string customerId = "CUST789";
        var items = new[]
        {
            new OrderItemsDto("SKU001", 2, 50.00m)
        };
        var request = new CreateOrderRequest(customerId, items);
        const decimal expectedTotal = 100.00m;

        _mockClassificationHelper.Setup(x => x.GetOrderClassification(expectedTotal))
            .Returns("HIGH");
        _mockWriteOrdersDb.Setup(x => x.CreateOrder(It.IsAny<OrderTable>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockClassificationHelper.Verify(x => x.GetOrderClassification(expectedTotal), Times.Once);
    }

    [Test]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsFailureResponse()
    {
        // Arrange
        const string customerId = "CUST123";
        var items = new[] { new OrderItemsDto("SKU001", 1, 25.00m) };
        var request = new CreateOrderRequest(customerId, items);

        _mockClassificationHelper.Setup(x => x.GetOrderClassification(It.IsAny<decimal>()))
            .Returns("LOW");
        _mockWriteOrdersDb.Setup(x => x.CreateOrder(It.IsAny<OrderTable>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.OrderId, Is.Null.Or.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleItems_MapsAllItemsCorrectly()
    {
        // Arrange
        const string customerId = "CUST999";
        var items = new[]
        {
            new OrderItemsDto("SKU001", 1, 10.00m),
            new OrderItemsDto("SKU002", 2, 20.00m),
            new OrderItemsDto("SKU003", 3, 30.00m)
        };
        var request = new CreateOrderRequest(customerId, items);

        _mockClassificationHelper.Setup(x => x.GetOrderClassification(It.IsAny<decimal>()))
            .Returns("HIGH");
        _mockWriteOrdersDb.Setup(x => x.CreateOrder(It.IsAny<OrderTable>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockWriteOrdersDb.Verify(
            x => x.CreateOrder(It.Is<OrderTable>(o =>
                o.Items.Length == 3 &&
                o.Items[0].Sku == "SKU001" && o.Items[0].Quantity == 1 &&
                o.Items[1].Sku == "SKU002" && o.Items[1].Quantity == 2 &&
                o.Items[2].Sku == "SKU003" && o.Items[2].Quantity == 3)),
            Times.Once);
    }

    [Test]
    public async Task Handle_OrderIdFormattedAs4Digits()
    {
        // Arrange
        const string customerId = "CUST123";
        var items = new[] { new OrderItemsDto("SKU001", 1, 25.00m) };
        var request = new CreateOrderRequest(customerId, items);

        _mockClassificationHelper.Setup(x => x.GetOrderClassification(It.IsAny<decimal>()))
            .Returns("LOW");
        _mockWriteOrdersDb.Setup(x => x.CreateOrder(It.IsAny<OrderTable>()))
            .ReturnsAsync(42);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.OrderId, Is.EqualTo("0042"));
    }
}






