using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderApp.Api.Requests;
using OrderApp.Data.Extensions;
using OrderApp.Query.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .RegisterConfiguration(builder.Configuration)
    .AddQueryServices()
    .AddDataServices()
    .AddValidation()
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly))
    .AddOpenApi()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/orders", async (IMediator mediatr, [FromBody]CreateOrderRequest request) =>
    {
        var createOrderResponse = await mediatr.Send(request).ConfigureAwait(false);
        
        return !string.IsNullOrWhiteSpace(createOrderResponse.OrderId)
            ? Results.Created($"/orders/{createOrderResponse.OrderId}", createOrderResponse)
            : Results.Problem("An error occurred while creating the order.");
    })
    .WithName("CreateOrder");

app.MapGet("/orders/{orderId}", async (IMediator mediatr, string orderId) =>
    {
        var getOrderResponse = await mediatr.Send(new GetOrderRequest(orderId)).ConfigureAwait(false);
        
        return getOrderResponse.OrderDetails is not null
            ? Results.Ok(getOrderResponse)
            : Results.NotFound($"Order {orderId} was not found.");
    })
    .WithName("GetOrder");

app.Run();