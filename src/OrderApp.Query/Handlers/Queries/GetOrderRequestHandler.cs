using MediatR;
using OrderApp.Api.Requests;
using OrderApp.Api.Responses;
using OrderApp.Data.Interfaces;

namespace OrderApp.Query.Handlers.Queries;

/// <summary>
/// Handles the <see cref="GetOrderRequest"/> request and collects the relevant order from the database.
/// </summary>
/// <param name="logger">The logger.</param>
/// <param name="readOrdersDb">The read orders interface.</param>
public partial class GetOrderRequestHandler(ILogger<GetOrderRequestHandler> logger, IReadOrdersDb readOrdersDb)
    : IRequestHandler<GetOrderRequest, GetOrderResponse>
{
    public async Task<GetOrderResponse> Handle(GetOrderRequest request, CancellationToken cancellationToken)
    {
        LogGetOrderHandlerCalled(request.OrderId);

        try
        {
            return new GetOrderResponse(await readOrdersDb.GetOrder(request.OrderId).ConfigureAwait(false));
        }
        catch (Exception e)
        {
            ErrorCollectingOrder(request.OrderId, e);
            return new GetOrderResponse(null);
        }
    }
    
    [LoggerMessage(LogLevel.Information, "Handling GetOrderRequest for order: {OrderId}")]
    partial void LogGetOrderHandlerCalled(int orderId);
    
    [LoggerMessage(LogLevel.Error, "Error collecting order: {OrderId}")]
    partial void ErrorCollectingOrder(int orderId, Exception exception);
}