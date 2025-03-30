public record OrderResponse
{
    public required Guid OrderId { get; init; }
    public required string CustomerName { get; init; }
    public required DateTimeOffset DateOrdered { get; init; }
};

public record CreateOrderRequest
{
    public required string CustomerName { get; init; }
}
