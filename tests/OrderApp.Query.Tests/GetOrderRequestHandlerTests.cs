using Microsoft.Extensions.Logging;
using Moq;
using OrderApp.Api.DataTransferObjects;
using OrderApp.Api.Requests;
using OrderApp.Data.Interfaces;
using OrderApp.Query.Handlers.Queries;

namespace OrderApp.Query.Tests;

public class GetOrderRequestHandlerTests
{
    private Mock<ILogger<GetOrderRequestHandler>> _mockLogger = null!;
    private Mock<IReadOrdersDb> _mockReadOrdersDb = null!;
    private GetOrderRequestHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<GetOrderRequestHandler>>();
        _mockReadOrdersDb = new Mock<IReadOrdersDb>();
        
        _handler = new GetOrderRequestHandler(_mockLogger.Object, _mockReadOrdersDb.Object);
    }

    [Test]
    public async Task Handle_WithExistingOrder_ReturnsOrderDetails()
    {
        // Arrange
        const int orderId = 1;
        var request = new GetOrderRequest(orderId);
        var expectedOrder = new OrderDetailsDto(
            "CUST123",
            [new OrderItemsDto("SKU001", 2, 25.00)],
            50.00,
            "LOW"
        );

        _mockReadOrdersDb.Setup(x => x.GetOrder(orderId))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.OrderDetails, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.OrderDetails.CustomerId, Is.EqualTo("CUST123"));
            Assert.That(result.OrderDetails.Total, Is.EqualTo(50.00));
            Assert.That(result.OrderDetails.Classification, Is.EqualTo("LOW"));
            Assert.That(result.OrderDetails.Items, Has.Length.EqualTo(1));
        });

        _mockReadOrdersDb.Verify(x => x.GetOrder(orderId), Times.Once);
    }

    [Test]
    public async Task Handle_WithNonExistentOrder_ReturnsNullOrderDetails()
    {
        // Arrange
        const int orderId = 9999;
        var request = new GetOrderRequest(orderId);

        _mockReadOrdersDb.Setup(x => x.GetOrder(orderId))
            .ReturnsAsync((OrderDetailsDto?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.OrderDetails, Is.Null);

        _mockReadOrdersDb.Verify(x => x.GetOrder(orderId), Times.Once);
    }

    [Test]
    public async Task Handle_WhenRepositoryThrowsException_ReturnsNullOrderDetails()
    {
        // Arrange
        const int orderId = 1;
        var request = new GetOrderRequest(orderId);

        _mockReadOrdersDb.Setup(x => x.GetOrder(orderId))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.OrderDetails, Is.Null);

        _mockReadOrdersDb.Verify(x => x.GetOrder(orderId), Times.Once);
    }

    [Test]
    public async Task Handle_CallsRepositoryWithCorrectOrderId()
    {
        // Arrange
        const int orderId = 123;
        var request = new GetOrderRequest(orderId);

        _mockReadOrdersDb.Setup(x => x.GetOrder(It.IsAny<int>()))
            .ReturnsAsync((OrderDetailsDto?)null);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mockReadOrdersDb.Verify(x => x.GetOrder(orderId), Times.Once);
    }

    [Test]
    public async Task Handle_WithMultipleItems_ReturnsAllOrderItems()
    {
        // Arrange
        const int orderId = 1;
        var request = new GetOrderRequest(orderId);
        var items = new[]
        {
            new OrderItemsDto("SKU001", 1, 10.00),
            new OrderItemsDto("SKU002", 2, 20.00),
            new OrderItemsDto("SKU003", 3, 30.00)
        };
        var expectedOrder = new OrderDetailsDto("CUST123", items, 140.00, "HIGH");

        _mockReadOrdersDb.Setup(x => x.GetOrder(orderId))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.OrderDetails!.Items, Has.Length.EqualTo(3));
            Assert.That(result.OrderDetails.Items[0].Sku, Is.EqualTo("SKU001"));
            Assert.That(result.OrderDetails.Items[1].Sku, Is.EqualTo("SKU002"));
            Assert.That(result.OrderDetails.Items[2].Sku, Is.EqualTo("SKU003"));
        });
    }

    [Test]
    public async Task Handle_ReturnsCorrectOrderClassification()
    {
        // Arrange
        const int orderId = 1;
        var request = new GetOrderRequest(orderId);
        var expectedOrder = new OrderDetailsDto(
            "CUST123",
            [new OrderItemsDto("SKU001", 1, 150.00)],
            150.00,
            "HIGH"
        );

        _mockReadOrdersDb.Setup(x => x.GetOrder(orderId))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.OrderDetails!.Classification, Is.EqualTo("HIGH"));
    }
}


