namespace Domain;

public record Order
{
    public required Guid OrderId { get; init; }
    public required string CustomerName { get; init; }
    public required DateTimeOffset DateOrdered { get; init; }
}
