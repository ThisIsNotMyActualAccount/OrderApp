using MediatR;
using OrderApp.Api.Requests;
using OrderApp.Api.Responses;
using OrderApp.Data.Interfaces;

namespace OrderApp.Query.Handlers.Queries;

public partial class GetOrderRequestHandler(ILogger<GetOrderRequestHandler> logger, IReadOrdersDb readOrdersDb)
    : IRequestHandler<GetOrderRequest, GetOrderResponse>
{
    public async Task<GetOrderResponse> Handle(GetOrderRequest request, CancellationToken cancellationToken)
    {
        LogGetOrderHandlerCalled(request.OrderId);
        
        if (!int.TryParse(request.OrderId, out var orderId))
        {
            LogOrderIdNotInteger(request.OrderId);
            return new GetOrderResponse(null);
        }

        try
        {
            return new GetOrderResponse(await readOrdersDb.GetOrder(orderId).ConfigureAwait(false));
        }
        catch (Exception e)
        {
            ErrorCollectingOrder(request.OrderId, e);
            return new GetOrderResponse(null);
        }
    }
    
    [LoggerMessage(LogLevel.Information, "Handling GetOrderRequest for order: {OrderId}")]
    partial void LogGetOrderHandlerCalled(string orderId);
    
    [LoggerMessage(LogLevel.Warning, "Order identifier does not translate to an integer: {OrderId}")]
    partial void LogOrderIdNotInteger(string orderId);
    
    [LoggerMessage(LogLevel.Error, "Error collecting order: {OrderId}")]
    partial void ErrorCollectingOrder(string orderId, Exception exception);
}