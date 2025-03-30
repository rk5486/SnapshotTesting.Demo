using Api;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using ZiggyCreatures.Caching.Fusion;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFusionCache();
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
builder.Services.AddExceptionHandler<ProblemExceptionHandler>();

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
       "/api/orders/{orderId:guid}", ([FromRoute] Guid orderId, IFusionCache cache) =>
       {
           Order? order = cache.GetOrDefault<Order>(orderId.ToString());

           if (order is null)
           {
               return Results.NotFound();
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
       "/api/orders", ([FromBody] CreateOrderRequest req, IFusionCache cache) =>
       {
           Order order = new()
           {
               OrderId = Guid.NewGuid(),
               CustomerName = req.CustomerName,
               DateOrdered = DateTimeOffset.UtcNow,
           };
           
           cache.Set(order.OrderId.ToString(), order);
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
