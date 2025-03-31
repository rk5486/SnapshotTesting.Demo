using Api;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure();
builder.Services.AddProblemDetails(
    opt =>
    {
        opt.CustomizeProblemDetails = ctx =>
        {
            ctx.ProblemDetails.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
            ctx.ProblemDetails.Extensions.TryAdd("requestId", ctx.HttpContext.TraceIdentifier);
            
            var activity = ctx.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
            ctx.ProblemDetails.Extensions.TryAdd("traceId", activity?.TraceId);
        };
    }
);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet(
       "/api/orders/{orderId:guid}", async ([FromRoute] Guid orderId, IOrderService orderService, CancellationToken ct) =>
       {
           Order? order = await orderService.GetOrderAsync(orderId, ct);

           if (order is null)
           {
               throw new NotFoundProblem($"Order {orderId} not found");
           }
           
           return Results.Ok(new OrderResponse
           {
               OrderId = order.OrderId,
               CustomerName = order.CustomerName,
               DateOrdered = order.DateOrdered,
           });
       }
   )
   .WithName("FetchOrderById")
   .WithOpenApi();

app.MapPost(
       "/api/orders", async ([FromBody] CreateOrderRequest req, IOrderService orderService, CancellationToken ct) =>
       {
           Order order = await orderService.CreateOrderAsync(req.CustomerName, ct);
           
           return Results.CreatedAtRoute("FetchOrderById", new { orderId = order.OrderId }, new OrderResponse
           {
               OrderId = order.OrderId,
               CustomerName = order.CustomerName,
               DateOrdered = order.DateOrdered,
           });
       }
   )
   .WithName("CreateOrder")
   .WithOpenApi();

app.MapGet(
       "/server-issue", () =>
       {
           throw new ApplicationException("Something went wrong");
       }
   )
   .WithName("ThrowServerError")
   .WithOpenApi();

app.Run();

public partial class Program;
