namespace Fe.Api.Features.CreateOrder;

public record CreateOrderRequest
{
    public required string CustomerName { get; init; }
}

public record CreateOrderResponse
{
    public required Guid OrderId { get; init; }
    public required string CustomerName { get; init; }
    public required DateTimeOffset DateOrdered { get; init; }
}
