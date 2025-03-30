using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet(
       "/api/orders/{orderId:guid}", ([FromRoute] Guid orderId) =>
       {
           return Results.Ok(new OrderResponse(orderId, "Mr Nobody",DateTimeOffset.Parse("30-Mar-25 1:19:06 AM +11:00")));
       }
   )
   .WithName("GetOrderById")
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

public record OrderResponse(Guid OrderId, string CustomerName, DateTimeOffset DateOrdered);
