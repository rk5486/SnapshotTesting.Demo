using FastEndpoints;
using Fe.Api.Features.FetchOrderById;
using Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Fe.Api.Features.CreateOrder;

public class CreateOrderEndpoint
    : Ep.Req<CreateOrderRequest>.Res<Results<CreatedAtRoute<CreateOrderResponse>, ProblemDetails>>
{
    private readonly IOrderService _orderService;

    public CreateOrderEndpoint(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public override void Configure()
    {
        Post("/orders");
    }

    public override async Task<Results<CreatedAtRoute<CreateOrderResponse>, ProblemDetails>> ExecuteAsync(
        CreateOrderRequest req, CancellationToken ct)
    {
        Domain.Order order = await _orderService.CreateOrderAsync(req.CustomerName, ct);

        CreateOrderResponse dto = new()
        {
            OrderId = order.OrderId,
            CustomerName = order.CustomerName,
            DateOrdered = order.DateOrdered,
        };

        return TypedResults.CreatedAtRoute(dto, "FetchOrderById", new { dto.OrderId });
    }
}
