using FastEndpoints;
using Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Fe.Api.Features.FetchOrderById;

public class FetchOrderByIdEndpoint
    : Ep.Req<FetchOrderByIdRequest>.Res<Results<Ok<FetchOrderByIdResponse>, NotFound, ProblemDetails>>
{
    public const string ROUTE_NAME = "FetchOrderById";
    
    private readonly IOrderService _orderService;
    
    public FetchOrderByIdEndpoint(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public override void Configure()
    {
        Get("/orders/{OrderId:guid}");
        Description(x => x.WithName(ROUTE_NAME));
    }

    public override async Task<Results<Ok<FetchOrderByIdResponse>, NotFound, ProblemDetails>> ExecuteAsync(
        FetchOrderByIdRequest req, CancellationToken ct)
    {
        await Task.Delay(500, ct);

        return TypedResults.Ok(
            new FetchOrderByIdResponse
                {
                    CustomerName = "CustomerName",
                    OrderId = req.OrderId,
                    DateOrdered = DateTime.UtcNow
                }
            );
    }
}
