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
app.RegisterMinimalApiEndpoints();
app.Run();