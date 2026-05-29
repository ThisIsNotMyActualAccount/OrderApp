using MediatR;
using OrderApp.Api.Requests;
using OrderApp.Api.Responses;
using OrderApp.Data.Interfaces;
using OrderApp.Data.Models;
using OrderApp.Query.Helpers;

namespace OrderApp.Query.Handlers.Commands;

public partial class CreateOrderRequestHandler(
    ILogger<CreateOrderRequestHandler> logger,
    IWriteOrdersDb writeOrdersDb,
    IOrderClassificationHelper orderClassificationHelper)
    : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
{
    public async Task<CreateOrderResponse> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        LogCreateOrderHandlerCalled(request.CustomerId);

        try
        {
            var total =  request.Items.Sum(orderItem => orderItem.Quantity * orderItem.Price);
            var classification = orderClassificationHelper.GetOrderClassification(total);
            
            var orderToBeAdded = new OrderTable
            {
                CustomerId = request.CustomerId,
                Items = request.Items.Select(orderItem => new OrderItemsTable
                {
                    Sku = orderItem.Sku,
                    Quantity = orderItem.Quantity,
                    Price = orderItem.Price,
                }).ToArray(),
                Total = total,
                Classification = classification,
            };
            
            var orderId = await writeOrdersDb.CreateOrder(orderToBeAdded).ConfigureAwait(false);
            
            return new CreateOrderResponse(orderId.ToString("0000"), orderToBeAdded.Total, orderToBeAdded.Classification);
        }
        catch (Exception e)
        {
            ErrorCreatingOrder(request.CustomerId, e);
            return new CreateOrderResponse();
        }
    }
    
    [LoggerMessage(LogLevel.Information, "Handling CreateOrderRequest for customer: {CustomerId}")]
    partial void LogCreateOrderHandlerCalled(string customerId);
    
    [LoggerMessage(LogLevel.Error, "Error creating order for customer: {CustomerId}")]
    partial void ErrorCreatingOrder(string customerId, Exception exception);
}